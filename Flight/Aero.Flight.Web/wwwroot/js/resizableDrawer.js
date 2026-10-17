// Drag-to-resize for the flight-detail drawer (right-docked = width, bottom-docked = height).
// Pure DOM manipulation while dragging for smooth feedback; the final size is reported back to
// .NET on release so it can be persisted (localStorage) and become the source of truth for render.
window.resizableDrawer = (function () {
    const instances = {};

    function attach(handleId, axis, minPx, maxPx, dotNetRef) {
        detach(handleId);

        const handle = document.getElementById(handleId);
        // Resolve via MudBlazor's own root class rather than a forwarded id — MudDrawer's
        // internal markup order isn't guaranteed to let a splatted "id" attribute win.
        const drawer = handle ? handle.closest('.mud-drawer') : null;
        if (!handle || !drawer) return;

        const state = { dragging: false, startPos: 0, startSize: 0 };

        const onPointerDown = (e) => {
            state.dragging = true;
            state.startPos = axis === 'height' ? e.clientY : e.clientX;
            const rect = drawer.getBoundingClientRect();
            state.startSize = axis === 'height' ? rect.height : rect.width;
            handle.setPointerCapture(e.pointerId);
            e.preventDefault();
        };

        const onPointerMove = (e) => {
            if (!state.dragging) return;
            const pos = axis === 'height' ? e.clientY : e.clientX;
            const delta = state.startPos - pos;
            const max = typeof maxPx === 'number' ? maxPx : (axis === 'height' ? window.innerHeight : window.innerWidth) * 0.9;
            const size = Math.max(minPx, Math.min(max, state.startSize + delta));
            drawer.style[axis] = size + 'px';
        };

        const onPointerUp = async () => {
            if (!state.dragging) return;
            state.dragging = false;
            const rect = drawer.getBoundingClientRect();
            const size = Math.round(axis === 'height' ? rect.height : rect.width);
            try {
                await dotNetRef.invokeMethodAsync('OnDrawerResized', axis, size);
            } catch {
                // Component may already be disposed (navigated away mid-drag) — ignore.
            }
        };

        handle.addEventListener('pointerdown', onPointerDown);
        window.addEventListener('pointermove', onPointerMove);
        window.addEventListener('pointerup', onPointerUp);

        instances[handleId] = { handle, onPointerDown, onPointerMove, onPointerUp };
    }

    function detach(handleId) {
        const instance = instances[handleId];
        if (!instance) return;

        instance.handle.removeEventListener('pointerdown', instance.onPointerDown);
        window.removeEventListener('pointermove', instance.onPointerMove);
        window.removeEventListener('pointerup', instance.onPointerUp);
        delete instances[handleId];
    }

    return { attach, detach };
})();
