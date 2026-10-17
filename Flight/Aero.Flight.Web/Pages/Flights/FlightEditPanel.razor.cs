using Aero.Flights.Web.Services;
using Aero.Flights.Web.Services.Flights;
using Aero.Flights.Web.Services.Metadatas;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;

namespace Aero.Flights.Web.Pages.Flights;

public partial class FlightEditPanel : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private const string ResourcesGridElementId = "flight-resources-grid";
    private const string ResourcesGridHandleElementId = "flight-resources-grid-handle";
    private const string ResourcesGridHeightStorageKey = "aeroflight.flight-resources-grid-height";
    private const int ResourcesGridDefaultHeightPx = 420;
    private const int ResourcesGridMinHeightPx = 220;

    // overflow:hidden (not auto) — the inner MudTable does its own internal scrolling once
    // Height/FixedHeader are set on it, so this box itself must never grow a scrollbar of its own.
    private string ResourcesGridStyle =>
        $"height:{ResourcesGridDefaultHeightPx}px; min-height:{ResourcesGridMinHeightPx}px; overflow:hidden; display:flex; flex-direction:column;";

    // Full-width bar instead of the browser's tiny native resize-corner grip (easy to miss once a
    // real scrollbar occupies that same corner) — same drag-handle pattern as resizableDrawer.js.
    private const string ResourcesGridHandleStyle =
        "height:10px; margin-top:2px; display:flex; align-items:center; justify-content:center; cursor:ns-resize;";

    private bool _resourcesGridAttached;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthState { get; set; }

    [Parameter]
    public string? Direction { get; set; }

    [Parameter]
    public long? Id { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback OnDeleted { get; set; }

    [Parameter]
    public EventCallback OnLinkChanged { get; set; }

    [Parameter]
    public EventCallback<(string Direction, long Id)> OnNavigateToLinked { get; set; }

    [Parameter]
    public bool HasPrevious { get; set; }

    [Parameter]
    public bool HasNext { get; set; }

    [Parameter]
    public EventCallback OnPrevious { get; set; }

    [Parameter]
    public EventCallback OnNext { get; set; }

    private MudForm _form = null!;
    private readonly DetailEditModel _model = new();
    private TimeZoneInfo _tenantTimeZone = TimeZoneInfo.Utc;
    private string _dateFormat = TenantDateTimeFormat.DefaultDateFormat;
    private string _timeFormat = TenantDateTimeFormat.DefaultTimeFormat;

    private bool _isArrival;
    private bool _loading = true;
    private bool _refreshing;
    private bool _notFound;
    private bool _saving;
    private bool _deleting;

    private string? _loadedDirection;
    private long? _loadedId;

    private ArrivalFlightDetail? _arrival;
    private DepartureFlightDetail? _departure;

    private string? _title;
    private string _airlineCode = "";
    private DateOnly _flightDate;
    private long? _linkedFlightId;
    private List<MetadataOption> _countryTypes = [];
    private List<MetadataOption> _flightTypes = [];
    private List<FlightStatusOption> _flightStatuses = [];
    private List<MetadataOption> _terminals = [];
    private IReadOnlyDictionary<string, string> _statusColors = new Dictionary<string, string>();

    private bool IsLocked => _flightStatuses.Any(s =>
        string.Equals(s.Code, _model.StatusCode, StringComparison.OrdinalIgnoreCase) && !s.IsUpdatable);

    private string GateResourceRoute => $"/api/flights/{(_isArrival ? "arrivals" : "departures")}/{Id}/gates";
    private string BeltResourceRoute => $"/api/flights/{(_isArrival ? "arrivals" : "departures")}/{Id}/{(_isArrival ? "arrivalbelts" : "departurebelts")}";
    private string CounterResourceRoute => $"/api/flights/departures/{Id}/counters";
    private string ParkPositionResourceRoute => $"/api/flights/{(_isArrival ? "arrivals" : "departures")}/{Id}/parkpositions";
    private string BeltMetadataRoute => MetadataCatalog.RouteOf(_isArrival ? "arrivalbelts" : "departurebelts");
    private string GateStatusMetadataRoute => MetadataCatalog.RouteOf("gatestatuses");
    private string BeltStatusMetadataRoute => MetadataCatalog.RouteOf(_isArrival ? "arrivalbeltstatuses" : "departurebeltstatuses");
    private string RoutingResourceRoute => $"/api/flights/{(_isArrival ? "arrivals" : "departures")}/{Id}/routings";
    private string CodeShareResourceRoute => $"/api/flights/{(_isArrival ? "arrivals" : "departures")}/{Id}/codeshares";
    private string DelayResourceRoute => $"/api/flights/{(_isArrival ? "arrivals" : "departures")}/{Id}/delays";
    private string MovementMessageResourceRoute => $"/api/flights/{(_isArrival ? "arrivals" : "departures")}/{Id}/movement-messages";
    private string ServiceResourceRoute => $"/api/flights/{(_isArrival ? "arrivals" : "departures")}/{Id}/services";

    protected override async Task OnParametersSetAsync()
    {
        if (Direction is null || Id is null)
        {
            _loadedDirection = null;
            _loadedId = null;
            return;
        }

        if (string.Equals(_loadedDirection, Direction, StringComparison.OrdinalIgnoreCase) && _loadedId == Id)
            return;

        _loadedDirection = Direction;
        _loadedId = Id;
        _resourcesGridAttached = false;

        if (AuthState is not null)
        {
            var state = await AuthState;
            _tenantTimeZone = TenantTimeZone.Resolve(state.User);
            _dateFormat = TenantDateTimeFormat.ResolveDateFormat(state.User);
            _timeFormat = TenantDateTimeFormat.ResolveTimeFormat(state.User);
        }

        _isArrival = string.Equals(Direction, "arrival", StringComparison.OrdinalIgnoreCase);
        var isDeparture = string.Equals(Direction, "departure", StringComparison.OrdinalIgnoreCase);
        if (!_isArrival && !isDeparture)
        {
            _loading = false;
            _notFound = true;
            return;
        }

        await LoadCountryTypesAsync();
        await LoadFlightTypesAsync();
        await LoadFlightStatusesAsync();
        await LoadStatusColorsAsync();
        await LoadTerminalsAsync();
        await LoadAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // The Resources tab's grid container renders inside a @key'd subtree that's torn down and
        // rebuilt on every flight switch, so re-attach until the element actually shows up in the DOM.
        if (_resourcesGridAttached || Direction is null || Id is null)
            return;

        try
        {
            _resourcesGridAttached = await JsRuntime.InvokeAsync<bool>(
                "resizableGrid.init", ResourcesGridElementId, ResourcesGridHandleElementId, ResourcesGridHeightStorageKey, ResourcesGridMinHeightPx);
        }
        catch (Exception ex) when (ex is JSDisconnectedException or InvalidOperationException)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("resizableGrid.dispose", ResourcesGridElementId);
        }
        catch (Exception ex) when (ex is JSDisconnectedException or InvalidOperationException)
        {
        }
    }

    private async Task LoadStatusColorsAsync()
    {
        try
        {
            _statusColors = await Flights.GetFlightStatusColorsAsync();
        }
        catch (Exception)
        {
            // Non-critical — StatusColor() falls back to Color.Success if this stays empty.
        }
    }

    private async Task LoadTerminalsAsync()
    {
        try
        {
            _terminals = await LoadMetadataOptionsAsync(MetadataCatalog.RouteOf("terminals"));
        }
        catch (Exception)
        {
            Snackbar.Add("Could not load Terminal options.", Severity.Warning);
        }
    }

    // Aircraft (~28k rows) and Airlines (~5.6k rows) are far too large for a fully-enumerated
    // dropdown — rendering every option freezes the page. These search the server (small paged
    // result) as the user types instead of preloading the whole Metadata table into the browser.
    private Task<IEnumerable<string>> SearchAircraftAsync(string search, CancellationToken ct) => SearchCodesAsync(MetadataCatalog.RouteOf("aircraft"), search, ct);
    private Task<IEnumerable<string>> SearchAircraftTypesAsync(string search, CancellationToken ct) => SearchCodesAsync(MetadataCatalog.RouteOf("aircrafttypes"), search, ct);
    private Task<IEnumerable<string>> SearchAirlinesAsync(string search, CancellationToken ct) => SearchCodesAsync(MetadataCatalog.RouteOf("airlines"), search, ct);

    // Origin/Destination and Divert all pick from the same airports list and share the same
    // code -> "Code — Name" label lookup, unlike the plain-code SearchCodesAsync above.
    private readonly Dictionary<string, string> _airportLabels = new();

    private Task<IEnumerable<string>> SearchAirportsAsync(string search, CancellationToken ct) =>
        MetadataCodeSearch.SearchWithLabelsAsync(MetadataData, MetadataCatalog.RouteOf("airports"), search, _airportLabels, ct);

    private async Task<IEnumerable<string>> SearchCodesAsync(string route, string search, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return [];

        try
        {
            // Cached full-list fetch (one HTTP/DB round trip per tenant, reused thereafter) —
            // every keystroke after the first just filters the already-in-memory list.
            var rows = await MetadataData.GetAllAsync(route, ct);
            IEnumerable<IReadOnlyDictionary<string, JsonElement>> matches = rows;

            // Empty search (field just opened, nothing typed yet) — show a browsable slice instead
            // of nothing, so clicking an empty combo isn't a dead end.
            if (!string.IsNullOrWhiteSpace(search))
                matches = matches.Where(row => RowMatches(row, search)).OrderBy(row => MatchRank(row, search));

            return matches
                .Select(row => row.TryGetValue("code", out var code) && code.ValueKind == JsonValueKind.String ? code.GetString() : null)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Take(20)!;
        }
        catch (OperationCanceledException)
        {
            return [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    // An exact/prefix code match must never be pushed out of the Take(20) cap by a pile of
    // looser name/description matches sorted ahead of it alphabetically (e.g. "SAW" the code
    // losing to "Warsaw"/"Massawa" in the name field) — rank code matches first.
    private static int MatchRank(IReadOnlyDictionary<string, JsonElement> row, string search)
    {
        var code = row.TryGetValue("code", out var c) && c.ValueKind == JsonValueKind.String ? c.GetString() : null;
        if (code is null) return 3;
        if (string.Equals(code, search, StringComparison.OrdinalIgnoreCase)) return 0;
        if (code.StartsWith(search, StringComparison.OrdinalIgnoreCase)) return 1;
        return 2;
    }

    // Matches on any string field in the row (code, name, displayName, etc.) so typing part of
    // an airline/airport/aircraft NAME finds it, not just typing the code.
    private static bool RowMatches(IReadOnlyDictionary<string, JsonElement> row, string search)
    {
        foreach (var value in row.Values)
        {
            if (value.ValueKind == JsonValueKind.String)
            {
                var s = value.GetString();
                if (!string.IsNullOrEmpty(s) && s.Contains(search, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }

    private async Task LoadCountryTypesAsync()
    {
        try
        {
            _countryTypes = await LoadMetadataOptionsAsync(MetadataCatalog.RouteOf("country-types"));
        }
        catch (Exception)
        {
            Snackbar.Add("Could not load International/Domestic options.", Severity.Warning);
        }
    }

    private async Task LoadFlightStatusesAsync()
    {
        try
        {
            var rows = await MetadataData.GetAllAsync(MetadataCatalog.RouteOf("flightstatuses"));
            var options = new List<FlightStatusOption>();
            foreach (var row in rows)
            {
                if (!row.TryGetValue("code", out var codeElement) || codeElement.ValueKind != JsonValueKind.String)
                    continue;

                var code = codeElement.GetString();
                if (string.IsNullOrWhiteSpace(code))
                    continue;

                var label = code;
                if (row.TryGetValue("description", out var descriptionElement) && descriptionElement.ValueKind == JsonValueKind.String)
                {
                    var description = descriptionElement.GetString();
                    if (!string.IsNullOrWhiteSpace(description))
                        label = $"{code} — {description}";
                }

                var isUpdatable = row.TryGetValue("isUpdatable", out var isUpdatableElement)
                    && isUpdatableElement.ValueKind == JsonValueKind.True;

                options.Add(new FlightStatusOption(code, label, isUpdatable));
            }

            _flightStatuses = options;
        }
        catch (Exception)
        {
            Snackbar.Add("Could not load Status options.", Severity.Warning);
        }
    }

    private async Task LoadFlightTypesAsync()
    {
        try
        {
            _flightTypes = await LoadMetadataOptionsAsync(MetadataCatalog.RouteOf("flighttypes"));
        }
        catch (Exception)
        {
            Snackbar.Add("Could not load Flight Type options.", Severity.Warning);
        }
    }

    private async Task<List<MetadataOption>> LoadMetadataOptionsAsync(string route)
    {
        var rows = await MetadataData.GetAllAsync(route);
        var options = new List<MetadataOption>();
        foreach (var row in rows)
        {
            if (!row.TryGetValue("code", out var codeElement) || codeElement.ValueKind != JsonValueKind.String)
                continue;

            var code = codeElement.GetString();
            if (string.IsNullOrWhiteSpace(code))
                continue;

            var label = code;
            if (row.TryGetValue("description", out var descriptionElement) && descriptionElement.ValueKind == JsonValueKind.String)
            {
                var description = descriptionElement.GetString();
                if (!string.IsNullOrWhiteSpace(description))
                    label = $"{code} — {description}";
            }

            options.Add(new MetadataOption(code, label));
        }

        return options;
    }
}
