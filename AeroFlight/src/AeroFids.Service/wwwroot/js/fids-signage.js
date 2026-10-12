// Server decides which images are active right now (date range, on/off) — see
// PublicSignageReader.GetActiveAsync. This just polls that decision and renders whatever comes
// back, rotating through the list on a timer. No date/schedule logic lives here.
window.FidsSignage = (function () {
    var DEFAULT_POLL_MS = 5 * 60 * 1000;
    var DEFAULT_ROTATE_MS = 10 * 1000;

    function attach(options) {
        var baseUrl = (options.baseUrl || '').replace(/\/$/, '');
        var tenant = options.tenant;
        var pollMs = options.pollMs || DEFAULT_POLL_MS;
        var rotateMs = options.rotateMs || DEFAULT_ROTATE_MS;
        var onActiveChange = options.onActiveChange || function () {};

        var overlay = document.createElement('div');
        overlay.style.cssText = 'position:fixed;inset:0;background:#000;display:none;z-index:999998;';
        var img = document.createElement('img');
        img.style.cssText = 'width:100%;height:100%;object-fit:contain;';
        overlay.appendChild(img);
        document.body.appendChild(overlay);

        var items = [];
        var index = 0;
        var rotateTimer = null;

        function showCurrent() {
            if (items.length === 0) return;
            img.src = baseUrl + '/api/signage/' + items[index].id + '/image?tenant=' + encodeURIComponent(tenant);
            index = (index + 1) % items.length;
        }

        function applyActive(newItems) {
            var wasActive = items.length > 0;
            items = newItems || [];
            var isActive = items.length > 0;

            if (isActive) {
                overlay.style.display = 'block';
                index = 0;
                showCurrent();
                if (!rotateTimer && items.length > 1)
                    rotateTimer = setInterval(showCurrent, rotateMs);
            } else {
                overlay.style.display = 'none';
                if (rotateTimer) {
                    clearInterval(rotateTimer);
                    rotateTimer = null;
                }
            }

            if (isActive !== wasActive) onActiveChange(isActive);
        }

        function poll() {
            fetch(baseUrl + '/api/signage/active?tenant=' + encodeURIComponent(tenant))
                .then(function (r) { return r.ok ? r.json() : []; })
                .then(applyActive)
                .catch(function () {});
        }

        poll();
        var pollTimer = setInterval(poll, pollMs);

        return {
            isActive: function () { return items.length > 0; },
            stop: function () {
                clearInterval(pollTimer);
                if (rotateTimer) clearInterval(rotateTimer);
                overlay.remove();
            }
        };
    }

    return { attach: attach };
})();
