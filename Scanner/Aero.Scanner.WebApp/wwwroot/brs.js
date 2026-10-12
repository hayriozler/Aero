window.brs = {
    applyTheme: function (isDark) {
        document.documentElement.classList.toggle('dark', isDark);
        localStorage.setItem('brs-theme', isDark ? 'dark' : 'light');
    },
    getTheme: function () {
        return localStorage.getItem('brs-theme');
    },
    getStorageItem: function (key) {
        return localStorage.getItem(key);
    },
    setStorageItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    setCulture: function (culture) {
        var cookieValue = 'c=' + culture + '|uic=' + culture;
        document.cookie = '.AspNetCore.Culture=' + encodeURIComponent(cookieValue)
            + ';path=/;max-age=31536000;samesite=strict';
        window.location.reload();
    },
    getUtcOffsetMinutes: function () {
        return -new Date().getTimezoneOffset();
    },
    printContainer: function (flight, container) {
        var w = window.open('', '_blank', 'width=860,height=700');
        if (!w) return;

        var d = w.document;
        d.open();
        d.write('<!DOCTYPE html><html lang="en"><head><meta charset="utf-8"><title>Container Label</title></head><body></body></html>');
        d.close();

        var st = d.createElement('style');
        st.textContent =
            'body{margin:0;font-family:Arial,sans-serif;display:flex;align-items:center;' +
            'justify-content:center;min-height:100vh;padding:2rem;box-sizing:border-box;background:#fff;}' +
            '.lbl{max-width:460px;width:100%;border:2px solid #000;padding:2rem;text-align:center;}' +
            '.flight-block{margin-bottom:1.25rem;padding-bottom:1rem;border-bottom:2px solid #000;}' +
            '.fn{font-size:3rem;font-weight:900;text-transform:uppercase;letter-spacing:.12em;line-height:1;}' +
            '.dst{font-size:1.4rem;font-weight:bold;margin-top:.4rem;color:#222;}' +
            '.container-block{display:flex;justify-content:center;gap:3rem;margin-bottom:1.5rem;' +
            'padding-bottom:1rem;border-bottom:1px solid #ccc;}' +
            '.itm{text-align:center;}' +
            '.itl{font-size:.6rem;text-transform:uppercase;letter-spacing:.1em;color:#888;margin-bottom:.25rem;}' +
            '.itv{font-size:1.4rem;font-weight:bold;}' +
            '.bw{margin-top:.5rem;}';
        d.head.appendChild(st);

        var lbl = d.createElement('div');
        lbl.className = 'lbl';
        lbl.innerHTML =
            '<div class="flight-block">' +
            '<div class="fn">' + flight.remoteSystemId + '</div>' +
            '<div class="dst">\u2192\u00a0' + flight.destinationAirport + '</div>' +
            '</div>' +
            '<div class="container-block">' +
            '<div class="itm"><div class="itl">Container</div><div class="itv">' + container.containerCode + '</div></div>' +
            '<div class="itm"><div class="itl">Class</div><div class="itv">' + container.containerClassCode + '</div></div>' +
            '</div>' +
            '<div class="bw"><svg id="brs-bc"></svg></div>';
        d.body.appendChild(lbl);

        var sc = d.createElement('script');
        sc.src = '/jsbarcode.all.min.js';
        sc.onload = function () {
            w.JsBarcode(d.getElementById('brs-bc'), String(container.id), {
                format: 'CODE128', displayValue: true,
                fontSize: 16, height: 120, width: 3, margin: 12
            });
            setTimeout(function () { w.print(); w.close(); }, 400);
        };
        d.head.appendChild(sc);
    }
};
