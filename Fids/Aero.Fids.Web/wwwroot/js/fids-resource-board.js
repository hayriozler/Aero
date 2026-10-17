// Pure rendering helpers for the resource-status demo pages (counters/gates/arrival-belts). The
// server (ResourceDatasetProjector.ProjectOverview) already groups resources by code, picks the
// status, and applies the FieldMapping rule — these pages just format the ResourceOverviewItem[]
// they're sent. No grouping, filtering, or status interpretation happens here.
window.FidsResourceBoard = (function () {
    function fmtDesignators(designators) {
        if (!designators || designators.length === 0) return '—';
        return designators.map(function (d) { return d.airlineCode + d.flightNumber; }).join(' / ');
    }

    // The client is a dumb renderer — the FIDS server already converted this to the airport's own
    // local wall-clock time (see Aero.Fids.Service's TenantLocalTime) and sends it as a plain "HH:mm"
    // string. Never reintroduce a Date/Intl timezone conversion here: this device has no idea what
    // timezone the airport it's displaying for is even in.
    function fmtTime(hhmm) {
        return hhmm || '—';
    }

    // Status is always exactly one of the server's closed InternalResourceStatuses vocabulary
    // (OPEN/CLOSED/UNKNOWN) — an exact match, never a guess at wording that could differ per tenant
    // or resource kind.
    function statusClass(status) {
        if (status === 'OPEN') return 'status-open';
        if (status === 'CLOSED') return 'status-closed';
        return 'status-neutral';
    }

    // URL templating only — whether a logo exists at that path/size is up to whatever files were
    // copied onto the FIDS server (see AirlineImageEndpoints); the browser's own <img onerror> is
    // the right place to hide a missing logo, not a check here.
    function airlineLogoUrl(baseUrl, airlineCode, size) {
        return (baseUrl || '').replace(/\/$/, '') + '/images/airlines/' + encodeURIComponent(airlineCode) + '/' + encodeURIComponent(size || 'sm');
    }

    return { fmtDesignators: fmtDesignators, fmtTime: fmtTime, statusClass: statusClass, airlineLogoUrl: airlineLogoUrl };
})();
