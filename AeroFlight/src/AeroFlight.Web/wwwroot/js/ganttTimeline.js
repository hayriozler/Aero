// Thin JS-interop wrapper around vis-timeline (Apache-2.0/MIT, vendored under wwwroot/lib/vis-timeline).
// No business logic here — groups/items are built server-/C#-side and passed in as plain objects.
window.ganttTimeline = (function () {
    const instances = {};

    // Purely diagnostic — observation only, never calls preventDefault, so it can't change drop
    // behavior. Confirms whether a 'drop' reaches the document at all when the container-scoped
    // listener in enableExternalDrop doesn't see it (e.g. an ancestor swallowed it first).
    if (!window.__ganttTimelineDocDropLogged) {
        window.__ganttTimelineDocDropLogged = true;
        document.addEventListener('drop', (e) => console.info('ganttTimeline[diag]: drop reached document', { target: e.target, defaultPrevented: e.defaultPrevented }));
        document.addEventListener('dragover', (e) => { if (!e.defaultPrevented && Math.random() < 0.05) console.debug('ganttTimeline[diag]: dragover reached document without preventDefault yet', { target: e.target }); });
    }

    function init(elementId, groups, items, options) {
        const container = document.getElementById(elementId);
        if (!container) {
            console.warn('ganttTimeline.init: no element with id', elementId);
            return;
        }

        dispose(elementId);

        const groupDataSet = new vis.DataSet(groups);
        const itemDataSet = new vis.DataSet(items);
        const timeline = new vis.Timeline(container, itemDataSet, groupDataSet, Object.assign({
            stack: true,
            zoomMin: 1000 * 60 * 15,
            zoomMax: 1000 * 60 * 60 * 36,
            orientation: 'top',
            tooltip: { followMouse: true, overflowMethod: 'flip' },
            // height must be a vis-timeline OPTION, not just CSS on the container — only then does
            // vis-timeline enable its own internal vertical scrollbar for the row/group list when
            // there are more rows than fit. Without it, the container's CSS `overflow: hidden`
            // (gantt-timeline-container) just hard-clips every row past the visible height with no
            // way to reach it at all — confirmed via a live DOM check: with 173 groups, every vis
            // panel was rendered at its full ~7800px content height and the outer container was the
            // only thing clipping it, with no scrollbar. verticalScroll makes this explicit rather
            // than relying on height alone to imply it.
            height: '600px',
            verticalScroll: true,
            snap: function (date) {
                const stepMs = 5 * 60 * 1000;
                return new Date(Math.round(date.getTime() / stepMs) * stepMs);
            },
        }, options || {}));

        instances[elementId] = { timeline, groupDataSet, itemDataSet };

        // Row/item heights are measured against whatever font is active at this point. The Barlow
        // webfont (Google Fonts) can still be loading here and swap in afterwards with different
        // metrics than the fallback — leaving item boxes taller than the row spacing computed
        // against the fallback font. Redraw once the real font is actually active to resync them.
        if (document.fonts && document.fonts.ready) {
            document.fonts.ready.then(() => {
                if (instances[elementId]) instances[elementId].timeline.redraw();
            });
        }
    }

    function updateItems(elementId, items) {
        const instance = instances[elementId];
        if (!instance) return;

        instance.itemDataSet.clear();
        instance.itemDataSet.add(items);
    }

    function updateGroups(elementId, groups) {
        const instance = instances[elementId];
        if (!instance) return;

        instance.groupDataSet.clear();
        instance.groupDataSet.add(groups);
    }

    // Formats a JS Date using its LOCAL getters (no timezone conversion) so it matches the naive
    // "wall clock" strings this module is fed elsewhere (updateItems/init) — using toISOString()
    // here would silently shift the time to UTC and desync from what's rendered on the timeline.
    function toWallClockString(date) {
        const pad = n => String(n).padStart(2, '0');
        return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
    }

    // Blazor's DragEventArgs/DataTransfer is a read-only snapshot with no SetData — the drag
    // payload has to be attached from plain JS. Delegated from the pool container (not each chip)
    // so it keeps working across Blazor re-renders that replace individual chip elements.
    //
    // timelineElementId's own pan/zoom is disabled for the duration of the drag — vis-timeline
    // treats any pointer movement over it as "pan the view" by default, which hijacks a drag
    // originating outside it (the drop lands on a panned grid instead of registering).
    function enablePoolDragSource(poolElementId, timelineElementId) {
        const pool = document.getElementById(poolElementId);
        if (!pool || pool.dataset.dragSourceAttached) return;
        pool.dataset.dragSourceAttached = 'true';

        pool.addEventListener('dragstart', (e) => {
            const chip = e.target.closest('.gantt-pool-chip');
            if (!chip) return;

            const payload = JSON.stringify({ flightId: Number(chip.dataset.flightId), direction: chip.dataset.direction });
            e.dataTransfer.setData('text/plain', payload);
            e.dataTransfer.effectAllowed = 'copy';
            chip.classList.add('gantt-pool-chip--dragging');

            // Without this the browser's default drag ghost is the pool chip itself, at its full
            // (much taller than a single row) size — swap in a compact badge sized like a Gantt bar.
            const code = chip.querySelector('.gantt-pool-chip__code')?.textContent || '';
            const ghost = document.createElement('div');
            ghost.textContent = code;
            ghost.style.cssText = 'position:absolute; top:-1000px; left:-1000px; padding:2px 8px; ' +
                'font:600 13px "Barlow Condensed",sans-serif; white-space:nowrap; border-radius:0; ' +
                'background:var(--color-accent,#b57a2c); color:var(--color-accent-100,#fffaee);';
            document.body.appendChild(ghost);
            e.dataTransfer.setDragImage(ghost, 12, 12);
            setTimeout(() => ghost.remove(), 0);

            const instance = instances[timelineElementId];
            if (instance) instance.timeline.setOptions({ moveable: false, zoomable: false });
        });

        pool.addEventListener('dragend', (e) => {
            const chip = e.target.closest('.gantt-pool-chip');
            if (chip) chip.classList.remove('gantt-pool-chip--dragging');

            const instance = instances[timelineElementId];
            if (instance) instance.timeline.setOptions({ moveable: true, zoomable: true });
        });
    }

    // Lets an already-placed bar be dragged to a different row (reassign) and/or a different time
    // (retime) — separate from enableExternalDrop, which only handles creating a NEW assignment by
    // dragging in from the unassigned pool. item.flightId/item.itemDirection are custom fields we
    // attach ourselves when building each item (see ResourceGantt.razor) — vis-timeline preserves
    // unknown fields on an item and hands the whole thing back here.
    function enableItemMove(elementId, dotNetRef) {
        const instance = instances[elementId];
        if (!instance) {
            console.warn('ganttTimeline.enableItemMove: no timeline instance for', elementId);
            return;
        }

        console.info('ganttTimeline.enableItemMove: editable drag enabled on', elementId);

        instance.timeline.setOptions({
            editable: { updateTime: true, updateGroup: true, remove: false, add: false },
            onMove: (item, callback) => {
                console.info('ganttTimeline: onMove fired', item);
                dotNetRef.invokeMethodAsync('OnResourceMovedAsync',
                    item.id, item.flightId, item.itemDirection, item.group,
                    toWallClockString(item.start), toWallClockString(item.end))
                    .then((ok) => callback(ok ? item : null))
                    .catch((err) => {
                        console.warn('ganttTimeline: OnResourceMovedAsync invoke failed.', err);
                        callback(null);
                    });
            },
        });
    }

    // One clipboard shared by every Gantt instance on the page — matches normal OS copy/paste
    // semantics (copy in one place, paste in another) rather than being scoped per-timeline.
    let clipboard = null;
    let openMenu = null;

    function closeContextMenu() {
        if (openMenu) {
            openMenu.remove();
            openMenu = null;
        }
    }

    function showContextMenu(x, y, entries) {
        closeContextMenu();

        const menu = document.createElement('div');
        menu.className = 'gantt-context-menu';
        menu.style.left = `${x}px`;
        menu.style.top = `${y}px`;

        for (const entry of entries) {
            const item = document.createElement('div');
            item.className = 'gantt-context-menu__item' + (entry.disabled ? ' gantt-context-menu__item--disabled' : '');
            item.textContent = entry.label;
            if (!entry.disabled) {
                item.addEventListener('click', (e) => {
                    e.stopPropagation();
                    closeContextMenu();
                    entry.onClick();
                });
            }
            menu.appendChild(item);
        }

        document.body.appendChild(menu);
        openMenu = menu;

        // Keep the menu on-screen if it was opened near the right/bottom edge.
        const rect = menu.getBoundingClientRect();
        if (rect.right > window.innerWidth) menu.style.left = `${Math.max(0, x - rect.width)}px`;
        if (rect.bottom > window.innerHeight) menu.style.top = `${Math.max(0, y - rect.height)}px`;
    }

    if (!window.__ganttTimelineMenuDismissWired) {
        window.__ganttTimelineMenuDismissWired = true;
        document.addEventListener('click', closeContextMenu);
        document.addEventListener('scroll', closeContextMenu, true);
        window.addEventListener('blur', closeContextMenu);
    }

    // Right-click on a bar: Copy (remember flight/direction/duration) or Delete (removes the
    // resource assignment via the same Delete endpoint the grid tables use). Right-click on empty
    // row space: Paste, if something's been copied — creates a new assignment for the copied
    // flight at the right-clicked row/time, same duration as the copied bar. The clipboard is
    // intentionally NOT tied to one resource kind/tab — pasting onto a different kind's timeline
    // reassigns the same flight to that kind instead, which is a reasonable, unsurprising outcome
    // rather than something worth blocking.
    function enableContextMenu(elementId, dotNetRef) {
        const instance = instances[elementId];
        if (!instance) {
            console.warn('ganttTimeline.enableContextMenu: no timeline instance for', elementId);
            return;
        }

        const container = document.getElementById(elementId);
        if (!container) {
            console.warn('ganttTimeline.enableContextMenu: no element with id', elementId);
            return;
        }

        const onContextMenu = (e) => {
            e.preventDefault();

            const props = instance.timeline.getEventProperties(e);
            if (props.item != null) {
                const itemData = instance.itemDataSet.get(props.item);
                if (!itemData) return;

                showContextMenu(e.clientX, e.clientY, [
                    {
                        label: 'Copy',
                        onClick: () => {
                            const start = itemData.start instanceof Date ? itemData.start : new Date(itemData.start);
                            const end = itemData.end instanceof Date ? itemData.end : new Date(itemData.end);
                            clipboard = {
                                flightId: itemData.flightId,
                                direction: itemData.itemDirection,
                                durationMs: end.getTime() - start.getTime(),
                            };
                        },
                    },
                    {
                        label: 'Delete',
                        onClick: () => {
                            dotNetRef.invokeMethodAsync('OnDeleteResourceAsync', itemData.id, itemData.flightId, itemData.itemDirection)
                                .catch((err) => console.warn('ganttTimeline: OnDeleteResourceAsync invoke failed.', err));
                        },
                    },
                ]);
                return;
            }

            const resolved = resolveDropGroup(instance, container, e);
            showContextMenu(e.clientX, e.clientY, [
                {
                    label: 'Paste',
                    disabled: !clipboard || !resolved,
                    onClick: () => {
                        if (!clipboard || !resolved) return;
                        const endTime = new Date(resolved.time.getTime() + clipboard.durationMs);
                        dotNetRef.invokeMethodAsync('OnPasteResourceAsync',
                            clipboard.flightId, clipboard.direction, resolved.group,
                            toWallClockString(resolved.time), toWallClockString(endTime))
                            .catch((err) => console.warn('ganttTimeline: OnPasteResourceAsync invoke failed.', err));
                    },
                },
            ]);
        };

        container.addEventListener('contextmenu', onContextMenu);
        instance.contextMenuHandler = { container, onContextMenu };
    }

    // vis-timeline's own getEventProperties(e).group is strict about landing exactly on a row's
    // rendered band — a drop a few px into the padding above/below the rows, or below the last
    // row when the container is taller than its content, resolves to no group and is silently
    // dropped. Falls back to whichever group's label is vertically closest to the pointer.
    function resolveDropGroup(instance, container, e) {
        const props = instance.timeline.getEventProperties(e);
        if (props.group) return { group: props.group, time: props.snappedTime || props.time };

        const labels = container.querySelectorAll('.vis-labelset .vis-label');
        if (!labels.length) return null;

        let bestIndex = null;
        let bestDistance = Infinity;
        labels.forEach((label, index) => {
            const rect = label.getBoundingClientRect();
            const distance = e.clientY < rect.top ? rect.top - e.clientY
                : e.clientY > rect.bottom ? e.clientY - rect.bottom
                : 0;
            if (distance < bestDistance) {
                bestDistance = distance;
                bestIndex = index;
            }
        });
        if (bestIndex === null) return null;

        const groups = instance.groupDataSet.get();
        const group = groups[bestIndex]?.id;
        if (!group) return null;

        return { group, time: props.snappedTime || props.time || instance.timeline.getCurrentTime() };
    }

    // Lets flights be dragged in from outside the timeline (e.g. an "unassigned flights" pool) and
    // dropped onto a resource row to create an assignment — payload is attached by
    // enablePoolDragSource above.
    //
    // Listeners are attached to the CONTAINER'S PARENT, not the container itself: the container is
    // wrapped in a BlueprintCard with visible padding (pa-2) that reads as part of the same drop
    // surface to a user, but a container-scoped listener never sees dragover there, so it's never
    // preventDefault()'d — and a drop landing in that padding (very plausible mid-drag) makes the
    // browser cancel the whole native drag instead of firing 'drop' at all. Confirmed via a
    // Playwright repro: dragover reached document targeting the BlueprintCard's own div, with
    // defaultPrevented still false, right before the drag silently died. Row/time resolution still
    // uses the inner container/instance — only the listen surface is widened.
    function enableExternalDrop(elementId, dotNetRef) {
        const instance = instances[elementId];
        if (!instance) {
            console.warn('ganttTimeline.enableExternalDrop: no timeline instance for', elementId, '— was init() called first / did it succeed?');
            return;
        }

        const container = document.getElementById(elementId);
        if (!container) {
            console.warn('ganttTimeline.enableExternalDrop: no element with id', elementId);
            return;
        }

        const dropTarget = container.parentElement || container;

        console.info('ganttTimeline.enableExternalDrop: listeners attached on', elementId, '(parent)', dropTarget);

        const onDragOver = (e) => {
            e.preventDefault();
            e.dataTransfer.dropEffect = 'copy';
            container.classList.add('gantt-drop-active');
        };
        const onDragLeave = () => {
            container.classList.remove('gantt-drop-active');
        };
        const onDrop = async (e) => {
            e.preventDefault();
            container.classList.remove('gantt-drop-active');
            console.info('ganttTimeline: drop event received', { clientX: e.clientX, clientY: e.clientY, target: e.target });

            const raw = e.dataTransfer.getData('text/plain');
            if (!raw) {
                console.warn('ganttTimeline: drop had no drag payload (text/plain empty).');
                return;
            }

            const resolved = resolveDropGroup(instance, container, e);
            if (!resolved) {
                console.warn('ganttTimeline: drop did not resolve to any group/row.', {
                    groups: instance.groupDataSet.get(),
                    labelCount: container.querySelectorAll('.vis-labelset .vis-label').length,
                });
                return;
            }

            let payload;
            try { payload = JSON.parse(raw); } catch (err) {
                console.warn('ganttTimeline: drop payload was not valid JSON.', raw, err);
                return;
            }

            try {
                await dotNetRef.invokeMethodAsync('OnFlightDroppedAsync',
                    payload.flightId, payload.direction, resolved.group, toWallClockString(resolved.time));
            } catch (err) {
                console.warn('ganttTimeline: OnFlightDroppedAsync invoke failed (component disposed?).', err);
            }
        };

        dropTarget.addEventListener('dragover', onDragOver);
        dropTarget.addEventListener('dragleave', onDragLeave);
        dropTarget.addEventListener('drop', onDrop);

        instance.dropHandlers = { dropTarget, onDragOver, onDragLeave, onDrop };
    }

    function dispose(elementId) {
        const instance = instances[elementId];
        if (!instance) return;

        if (instance.dropHandlers) {
            const { dropTarget, onDragOver, onDragLeave, onDrop } = instance.dropHandlers;
            dropTarget.removeEventListener('dragover', onDragOver);
            dropTarget.removeEventListener('dragleave', onDragLeave);
            dropTarget.removeEventListener('drop', onDrop);
        }

        if (instance.contextMenuHandler) {
            const { container, onContextMenu } = instance.contextMenuHandler;
            container.removeEventListener('contextmenu', onContextMenu);
        }
        closeContextMenu();

        instance.timeline.destroy();
        delete instances[elementId];
    }

    return {
        init, updateItems, updateGroups, enablePoolDragSource, enableExternalDrop, enableItemMove,
        enableContextMenu, dispose,
    };
})();
