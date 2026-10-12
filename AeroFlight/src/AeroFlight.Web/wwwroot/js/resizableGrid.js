// Native CSS resize:vertical only exposes a ~12px grip inside the scroll-corner of the element,
// which is easy to miss/mis-hit once a real scrollbar occupies the same corner — replaced with a
// full-width drag handle (same pattern as resizableDrawer.js).
//
// MudTabs only mounts the ACTIVE panel's content into the DOM; switching tabs is handled entirely
// inside MudTabs' own component state and does not re-run this page's OnAfterRenderAsync, so a
// single init() call made before the Resources tab is ever opened would never find the element and
// would never get a second chance to. A MutationObserver — not a Blazor-triggered retry — is what
// notices the grid/handle (re)appearing (including being torn down and recreated on later tab
// switches) and (re)attaches the drag listeners.
window.resizableGrid = (function () {
    const instances = {};

    function attachListeners(el, handle, storageKey, minPx, maxPx) {
        try {
            const stored = window.localStorage.getItem(storageKey);
            const storedPx = stored ? parseInt(stored, 10) : NaN;
            if (!Number.isNaN(storedPx) && storedPx >= minPx) {
                el.style.height = storedPx + 'px';
            }
        } catch {
        }

        const state = { el, handle, dragging: false, startY: 0, startHeight: 0 };

        state.onPointerDown = (e) => {
            state.dragging = true;
            state.startY = e.clientY;
            state.startHeight = el.getBoundingClientRect().height;
            handle.setPointerCapture(e.pointerId);
            e.preventDefault();
        };

        state.onPointerMove = (e) => {
            if (!state.dragging) return;
            const delta = e.clientY - state.startY;
            const max = typeof maxPx === 'number' ? maxPx : Infinity;
            const height = Math.max(minPx, Math.min(max, state.startHeight + delta));
            el.style.height = height + 'px';
        };

        state.onPointerUp = () => {
            if (!state.dragging) return;
            state.dragging = false;
            try {
                window.localStorage.setItem(storageKey, Math.round(el.getBoundingClientRect().height).toString());
            } catch {
            }
        };

        handle.addEventListener('pointerdown', state.onPointerDown);
        window.addEventListener('pointermove', state.onPointerMove);
        window.addEventListener('pointerup', state.onPointerUp);
        return state;
    }

    function detachListeners(state) {
        state.handle.removeEventListener('pointerdown', state.onPointerDown);
        window.removeEventListener('pointermove', state.onPointerMove);
        window.removeEventListener('pointerup', state.onPointerUp);
    }

    function init(elementId, handleId, storageKey, minPx, maxPx) {
        let instance = instances[elementId];
        if (!instance) {
            instance = { state: null, observer: null };
            instances[elementId] = instance;
        }

        const sync = () => {
            const el = document.getElementById(elementId);
            const handle = document.getElementById(handleId);
            const stale = instance.state
                && (instance.state.el !== el || instance.state.handle !== handle || !document.body.contains(instance.state.el));

            if (instance.state && stale) {
                detachListeners(instance.state);
                instance.state = null;
            }

            if (!instance.state && el && handle) {
                instance.state = attachListeners(el, handle, storageKey, minPx, maxPx);
            }
        };

        sync();

        if (!instance.observer) {
            instance.observer = new MutationObserver(sync);
            instance.observer.observe(document.body, { childList: true, subtree: true });
        }

        return !!instance.state;
    }

    function dispose(elementId) {
        const instance = instances[elementId];
        if (!instance) return;
        if (instance.observer) instance.observer.disconnect();
        if (instance.state) detachListeners(instance.state);
        delete instances[elementId];
    }

    return { init, dispose };
})();
