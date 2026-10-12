// Positions the Arrival/Departure banner bars above the movements grid's own header row.
// MudDataGrid renders a single header <tr> with no built-in multi-row/group-header support
// (HeaderColSpan only widens that column's own <th>, it does not suppress the neighboring
// columns' cells), so the banner is a hand-built overlay kept in sync via ResizeObserver
// against the real header cell rects (column widths change on resize/reflow, not reorder —
// this grid has ColumnResizeMode.Column but DragDropColumnReordering is off).
window.movementsGroupHeader = (function () {
    const instances = {};

    function applyBarRect(bar, cells, containerRect) {
        if (!bar) return;
        if (!cells || cells.length === 0) {
            bar.style.display = 'none';
            return;
        }

        let left = Infinity;
        let right = -Infinity;
        cells.forEach((cell) => {
            const rect = cell.getBoundingClientRect();
            left = Math.min(left, rect.left);
            right = Math.max(right, rect.right);
        });

        bar.style.display = 'flex';
        bar.style.left = (left - containerRect.left) + 'px';
        bar.style.width = Math.max(0, right - left) + 'px';
    }

    function init(containerId) {
        dispose(containerId);

        const container = document.getElementById(containerId);
        if (!container) return;

        const arrivalBar = container.querySelector('.movements-group-banner-arrival');
        const departureBar = container.querySelector('.movements-group-banner-departure');

        const run = () => {
            const containerRect = container.getBoundingClientRect();
            const arrivalCells = container.querySelectorAll('.movements-group-arrival');
            const departureCells = container.querySelectorAll('.movements-group-departure');
            applyBarRect(arrivalBar, arrivalCells, containerRect);
            applyBarRect(departureBar, departureCells, containerRect);
        };

        const observedCells = container.querySelectorAll('.movements-group-arrival, .movements-group-departure');
        const observer = new ResizeObserver(run);
        observedCells.forEach((cell) => observer.observe(cell));
        observer.observe(container);
        window.addEventListener('resize', run);

        instances[containerId] = { observer, run };
        run();
    }

    function dispose(containerId) {
        const instance = instances[containerId];
        if (!instance) return;
        instance.observer.disconnect();
        window.removeEventListener('resize', instance.run);
        delete instances[containerId];
    }

    return { init, dispose };
})();
