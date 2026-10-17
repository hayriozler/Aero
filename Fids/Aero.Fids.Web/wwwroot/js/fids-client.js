window.FidsClient = (function () {
    var DEFAULT_DISCONNECT_ALERT_MS = 2 * 60 * 1000;
    var RECONNECT_RETRY_DELAYS_MS = [2000, 5000, 10000, 30000];
    var overlayEl = null;

    function ensureOverlay() {
        if (overlayEl) return overlayEl;
        var style = document.createElement('style');
        style.textContent =
            '@keyframes fids-disconnect-flash { 0%, 100% { opacity: 1; } 50% { opacity: 0.15; } }' +
            '#fids-disconnect-overlay { position: fixed; inset: 0; border: 14px solid #ff1414; box-sizing: border-box; ' +
            'z-index: 999999; pointer-events: none; animation: fids-disconnect-flash 1s step-start infinite; }' +
            '#fids-disconnect-overlay span { position: absolute; top: 20px; left: 50%; transform: translateX(-50%); ' +
            'background: #ff1414; color: #fff; font: 700 16px system-ui, sans-serif; padding: 6px 18px; ' +
            'border-radius: 4px; letter-spacing: 0.05em; }';
        document.head.appendChild(style);
        overlayEl = document.createElement('div');
        overlayEl.id = 'fids-disconnect-overlay';
        overlayEl.innerHTML = '<span>CONNECTION LOST</span>';
        overlayEl.style.display = 'none';
        document.body.appendChild(overlayEl);
        return overlayEl;
    }

    var RESOURCE_EVENTS = ['gateUpdated', 'beltUpdated', 'counterUpdated', 'spotUpdated'];

    function connect(options) {
        var baseUrl = (options.baseUrl || '').replace(/\/$/, '');
        var tenant = options.tenant;
        var name = options.name;
        var debug = options.debug !== false;
        var disconnectAlertMs = options.disconnectAlertMs || DEFAULT_DISCONNECT_ALERT_MS;
        var onUpdate = options.onUpdate || function () {};
        var onError = options.onError || function () {};
        var onStateChange = options.onStateChange || function () {};

        var disconnectTimer = null;

        function startDisconnectTimerIfNeeded() {
            if (disconnectTimer) return;
            disconnectTimer = setTimeout(function () {
                disconnectTimer = null;
                ensureOverlay().style.display = 'block';
                log('disconnect alert shown — no connection for ' + disconnectAlertMs + 'ms');
            }, disconnectAlertMs);
        }

        function clearDisconnectState() {
            if (disconnectTimer) {
                clearTimeout(disconnectTimer);
                disconnectTimer = null;
            }
            if (overlayEl) overlayEl.style.display = 'none';
        }

        function handleStateChange(state) {
            if (state === 'connected') clearDisconnectState();
            else startDisconnectTimerIfNeeded();
            onStateChange(state);
        }

        function log() {
            if (!debug) return;
            var args = Array.prototype.slice.call(arguments);
            args.unshift('[FidsClient ' + new Date().toISOString() + ']');
            console.log.apply(console, args);
        }

        function logError(message, err) {
            console.error('[FidsClient ' + new Date().toISOString() + '] ' + message, err || '');
        }

        if (!tenant || !name) {
            var configError = new Error('FidsClient.connect requires both "tenant" and "name" — the server resolves what this screen shows from its registered display client.');
            logError('bad configuration', configError);
            throw configError;
        }

        log('connect() called', { baseUrl: baseUrl, tenant: tenant, name: name });

        var itemsById = new Map();

        function notify(reason) {
            var items = Array.from(itemsById.values()).sort(function (a, b) {
                return new Date(a.scheduledTime) - new Date(b.scheduledTime);
            });
            log('notify (' + reason + ') — ' + items.length + ' item(s) on board');
            onUpdate(items);
        }

        // Flight-list items are one row per flight (flightId alone is unique). Single-instance
        // resource-board items (Gate/Counter/*Belt/Spot) are one row per (flight, resource code) — a
        // flight can have more than one gate, so the resource code (whichever of gateCode/counterCode/
        // beltCode/spotCode/the bare "code" a removal notice carries is present) is folded into the
        // key too. Overview items (ResourceOverviewItem — one row per code, already grouped
        // server-side) have a "code" but no flightId at all, so the code alone is their key.
        function resourceCodeOf(item) {
            return item.gateCode || item.counterCode || item.beltCode || item.spotCode || item.code;
        }

        function keyFor(item) {
            var resourceCode = resourceCodeOf(item);
            if (!resourceCode) return String(item.flightId);
            return item.flightId === undefined ? String(resourceCode) : (item.flightId + ':' + resourceCode);
        }

        function applySnapshot(items) {
            itemsById.clear();
            items.forEach(function (item) { itemsById.set(keyFor(item), item); });
            log('snapshot applied — ' + items.length + ' item(s)', items);
            notify('snapshot');
        }

        // A resource board (Gate/Counter/*Belt/Spot) delta can be a real dataset item, or a removal
        // notice ({ flightId, code }, no "designators") sent when a flight no longer carries that
        // resource at all (e.g. its gate was reassigned) — the row is dropped instead of upserted.
        function applyDelta(item) {
            if (!item) return;
            var key = keyFor(item);
            if (item.designators === undefined) {
                log('resource unassigned — removing', item);
                itemsById.delete(key);
                notify('removed:' + key);
                return;
            }
            log('delta received', item);
            itemsById.set(key, item);
            notify('delta:' + key);
        }

        var hubUrl = baseUrl + '/hubs/board?tenant=' + encodeURIComponent(tenant) + '&name=' + encodeURIComponent(name);

        var stopped = false;
        var currentConnection = null;
        var reconnectTimer = null;
        var reconnectAttempt = 0;

        function scheduleReconnect() {
            if (stopped || reconnectTimer) return;

            var delay = RECONNECT_RETRY_DELAYS_MS[Math.min(reconnectAttempt, RECONNECT_RETRY_DELAYS_MS.length - 1)];
            reconnectAttempt++;
            log('will retry connect() in ' + delay + 'ms (attempt ' + reconnectAttempt + ')');
            reconnectTimer = setTimeout(function () {
                reconnectTimer = null;
                attemptConnect();
            }, delay);
        }

        function attemptConnect() {
            if (stopped) return Promise.resolve();

            log('building hub connection', hubUrl);
            var connection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl)
                .withAutomaticReconnect()
                .configureLogging(debug ? signalR.LogLevel.Information : signalR.LogLevel.Warning)
                .build();
            currentConnection = connection;

            connection.on('boardSnapshot', applySnapshot);
            connection.on('flightUpdated', applyDelta);
            RESOURCE_EVENTS.forEach(function (evt) { connection.on(evt, applyDelta); });
            connection.on('boardError', function (message) {
                logError('server rejected this board connection', message);
                onError(new Error(message));
            });
            connection.onreconnecting(function (err) {
                logError('SignalR reconnecting (connection dropped)', err);
                handleStateChange('reconnecting');
            });
            connection.onreconnected(function (connectionId) {
                log('SignalR reconnected', connectionId);
                reconnectAttempt = 0;
                handleStateChange('connected');
            });
            connection.onclose(function (err) {
                // Fires both on a clean stop() and once SignalR's own automatic-reconnect gives up
                // for good — only the latter should trigger our own outer retry loop.
                if (stopped) return;
                if (err) logError('SignalR connection closed unexpectedly', err);
                else log('SignalR connection closed');
                handleStateChange('closed');
                scheduleReconnect();
            });

            log('starting SignalR connection...');
            return connection.start()
                .then(function () {
                    log('SignalR connected', { connectionId: connection.connectionId });
                    reconnectAttempt = 0;
                    handleStateChange('connected');
                })
                .catch(function (err) {
                    logError('connect() failed — is Aero.Fids.Service running and reachable at "' + (baseUrl || '(same origin)') + '"? Check the tenant id and the display client name you passed.', err);
                    handleStateChange('failed');
                    onError(err);
                    scheduleReconnect();
                });
        }

        startDisconnectTimerIfNeeded();
        var ready = attemptConnect();

        return {
            ready: ready,
            stop: function () {
                log('stop() called');
                stopped = true;
                if (reconnectTimer) {
                    clearTimeout(reconnectTimer);
                    reconnectTimer = null;
                }
                clearDisconnectState();
                return currentConnection ? currentConnection.stop() : Promise.resolve();
            },
            getState: function () {
                return currentConnection ? currentConnection.state : 'Disconnected';
            },
            isDisconnectAlertVisible: function () {
                return !!overlayEl && overlayEl.style.display !== 'none';
            },
            triggerDisconnectAlertNow: function () {
                clearTimeout(disconnectTimer);
                disconnectTimer = null;
                ensureOverlay().style.display = 'block';
            }
        };
    }

    return { connect: connect };
})();
