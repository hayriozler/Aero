using Aero.Flights.Contracts.Flights;
using Aero.Flights.Web.Services.Metadatas;

namespace Aero.Flights.Web.Services.Flights;

public interface IFlightsClient
{
    Task<FlightResponse> CreateDepartureAsync(DepartureCreateRequest request, CancellationToken ct = default);
    Task<FlightResponse> CreateArrivalAsync(ArrivalCreateRequest request, CancellationToken ct = default);

    Task<FlightsPageResult> GetFlightsAsync(
        IReadOnlyList<string>? directions = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        string? search = null,
        IReadOnlyList<string>? terminalCodes = null,
        IReadOnlyList<string>? countryTypes = null,
        string? airlineCode = null,
        IReadOnlyList<string>? statuses = null,
        int page = 1,
        int? pageSize = null,
        CancellationToken ct = default);

    Task<ArrivalFlightDetail> GetArrivalDetailAsync(long id, CancellationToken ct = default);
    Task<DepartureFlightDetail> GetDepartureDetailAsync(long id, CancellationToken ct = default);
    Task<FlightResponse> UpdateArrivalAsync(long id, UpdateArrivalRequest request, CancellationToken ct = default);
    Task<FlightResponse> UpdateDepartureAsync(long id, UpdateDepartureRequest request, CancellationToken ct = default);
    Task DeleteArrivalAsync(long id, bool deleteLinkedFlight = false, CancellationToken ct = default);
    Task DeleteDepartureAsync(long id, bool deleteLinkedFlight = false, CancellationToken ct = default);
    Task UnlinkArrivalAsync(long id, CancellationToken ct = default);
    Task UnlinkDepartureAsync(long id, CancellationToken ct = default);
    Task LinkArrivalAsync(long id, long departureId, CancellationToken ct = default);
    Task LinkDepartureAsync(long id, long arrivalId, CancellationToken ct = default);

    Task<IReadOnlyList<GanttItem>> GetGanttItemsAsync(string type, DateOnly fromDate, DateOnly toDate, CancellationToken ct = default);
    Task<OpsOverview> GetOpsOverviewAsync(DateOnly date, CancellationToken ct = default);
    Task<IReadOnlyList<UnassignedFlightItem>> GetUnassignedFlightsAsync(string type, DateOnly fromDate, DateOnly toDate, CancellationToken ct = default);

    Task<IReadOnlyList<AuditEntryDto>> GetArrivalAuditAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<AuditEntryDto>> GetDepartureAuditAsync(long id, CancellationToken ct = default);

    Task<IReadOnlyList<FlightResourceResponse>> GetFlightResourcesAsync(string route, CancellationToken ct = default);
    Task<FlightResourceResponse> AddFlightResourceAsync(string route, FlightResourceRequest request, CancellationToken ct = default);
    Task<FlightResourceResponse> UpdateFlightResourceAsync(string route, FlightResourceRequest request, CancellationToken ct = default);
    Task DeleteFlightResourceAsync(string route, CancellationToken ct = default);
    Task<IReadOnlyList<FlightResourceResponse>> BulkUpdateFlightResourceStatusAsync(string routeBase, string statusCode, CancellationToken ct = default);

    Task<IReadOnlyList<FlightResourceResponse>> GetCounterResourcesAsync(string route, CancellationToken ct = default);
    Task<FlightResourceResponse> AddCounterResourceAsync(string route, FlightResourceRequest request, CancellationToken ct = default);
    Task<FlightResourceResponse> UpdateCounterResourceAsync(string route, FlightResourceRequest request, CancellationToken ct = default);
    Task DeleteCounterResourceAsync(string route, CancellationToken ct = default);
    Task<IReadOnlyList<FlightResourceResponse>> BulkUpdateCounterResourceStatusAsync(string routeBase, string statusCode, CancellationToken ct = default);
    Task<int> GetResourceDefaultDurationMinutesAsync(string kind, CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, string>> GetFlightStatusColorsAsync(CancellationToken ct = default);
    Task<string> GetResourceGridDateFormatAsync(CancellationToken ct = default);

    Task<IReadOnlyList<OpsRoutingResponse>> GetRoutingsAsync(string route, CancellationToken ct = default);
    Task<OpsRoutingResponse> AddRoutingAsync(string route, OpsRoutingRequest request, CancellationToken ct = default);
    Task<OpsRoutingResponse> UpdateRoutingAsync(string route, OpsRoutingRequest request, CancellationToken ct = default);
    Task DeleteRoutingAsync(string route, CancellationToken ct = default);

    Task<IReadOnlyList<OpsParkPositionResponse>> GetParkPositionsAsync(string route, CancellationToken ct = default);
    Task<OpsParkPositionResponse> AddParkPositionAsync(string route, OpsParkPositionRequest request, CancellationToken ct = default);
    Task<OpsParkPositionResponse> UpdateParkPositionAsync(string route, OpsParkPositionRequest request, CancellationToken ct = default);
    Task DeleteParkPositionAsync(string route, CancellationToken ct = default);

    Task<IReadOnlyList<CodeShareResponse>> GetCodeSharesAsync(string route, CancellationToken ct = default);
    Task<CodeShareResponse> AddCodeShareAsync(string route, CodeShareRequest request, CancellationToken ct = default);
    Task<CodeShareResponse> UpdateCodeShareAsync(string route, CodeShareRequest request, CancellationToken ct = default);
    Task DeleteCodeShareAsync(string route, CancellationToken ct = default);

    Task<IReadOnlyList<DelayResponse>> GetDelaysAsync(string route, CancellationToken ct = default);
    Task<IReadOnlyList<MovementMessageResponse>> GetMovementMessagesAsync(string route, CancellationToken ct = default);
    Task<DelayResponse> AddDelayAsync(string route, DelayRequest request, CancellationToken ct = default);
    Task<DelayResponse> UpdateDelayAsync(string route, DelayRequest request, CancellationToken ct = default);
    Task DeleteDelayAsync(string route, CancellationToken ct = default);

    Task<IReadOnlyList<ServiceResponse>> GetServicesAsync(string route, CancellationToken ct = default);
    Task<ServiceResponse> AddServiceAsync(string route, ServiceRequest request, CancellationToken ct = default);
    Task<ServiceResponse> UpdateServiceAsync(string route, ServiceRequest request, CancellationToken ct = default);
    Task DeleteServiceAsync(string route, CancellationToken ct = default);

    Task<bool> UpdateFlightStatusAsync(string direction, long id, string statusCode, CancellationToken ct = default);
    Task<bool> UpdateFlightTerminalAsync(string direction, long id, string? terminalCode, CancellationToken ct = default);
    Task<bool> UpdateFlightRegistrationAsync(string direction, long id, string? aircraftCode, CancellationToken ct = default);
}

public sealed class FlightsClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IFlightsClient
{
    private const string _route = "/api/flights";

    public Task<FlightResponse> CreateDepartureAsync(DepartureCreateRequest request, CancellationToken ct = default)
        => PostForResultAsync<FlightResponse>($"{_route}/departures", request, ct);

    public Task<FlightResponse> CreateArrivalAsync(ArrivalCreateRequest request, CancellationToken ct = default)
        => PostForResultAsync<FlightResponse>($"{_route}/arrivals", request, ct);

    public async Task<FlightsPageResult> GetFlightsAsync(
        IReadOnlyList<string>? directions = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        string? search = null,
        IReadOnlyList<string>? terminalCodes = null,
        IReadOnlyList<string>? countryTypes = null,
        string? airlineCode = null,
        IReadOnlyList<string>? statuses = null,
        int page = 1,
        int? pageSize = null,
        CancellationToken ct = default)
    {
        var query = new List<string>();
        if (directions is { Count: > 0 })
        {
            foreach (var direction in directions)
                query.Add($"direction={Uri.EscapeDataString(direction)}");
        }
        if (fromDate is { } from)
            query.Add($"fromDate={from:yyyy-MM-dd}");
        if (toDate is { } to)
            query.Add($"toDate={to:yyyy-MM-dd}");
        if (!string.IsNullOrWhiteSpace(search))
            query.Add($"search={Uri.EscapeDataString(search)}");
        if (terminalCodes is { Count: > 0 })
        {
            foreach (var terminalCode in terminalCodes)
                query.Add($"terminalCode={Uri.EscapeDataString(terminalCode)}");
        }
        if (countryTypes is { Count: > 0 })
        {
            foreach (var countryType in countryTypes)
                query.Add($"countryType={Uri.EscapeDataString(countryType)}");
        }
        if (!string.IsNullOrWhiteSpace(airlineCode))
            query.Add($"airlineCode={Uri.EscapeDataString(airlineCode)}");
        if (statuses is { Count: > 0 })
        {
            foreach (var status in statuses)
                query.Add($"statuses={Uri.EscapeDataString(status)}");
        }
        query.Add($"page={page}");
        if (pageSize is { } size)
            query.Add($"pageSize={size}");

        var uri = query.Count == 0 ? _route : $"{_route}?{string.Join('&', query)}";
        using var request = CreateRequest(HttpMethod.Get, uri);
        using var response = await Http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<MergedFlightRow>>>(JsonOptions, ct);
        var items = envelope?.Data ?? [];
        var pagination = envelope?.Pagination;
        return new FlightsPageResult(
            items,
            pagination?.Page ?? page,
            pagination?.PageSize ?? items.Count,
            pagination?.TotalCount ?? items.Count,
            pagination?.TotalPages ?? 1);
    }

    public Task<ArrivalFlightDetail> GetArrivalDetailAsync(long id, CancellationToken ct = default)
        => GetAsync<ArrivalFlightDetail>($"{_route}/arrivals/{id}", ct);

    public Task<DepartureFlightDetail> GetDepartureDetailAsync(long id, CancellationToken ct = default)
        => GetAsync<DepartureFlightDetail>($"{_route}/departures/{id}", ct);

    public Task<FlightResponse> UpdateArrivalAsync(long id, UpdateArrivalRequest request, CancellationToken ct = default)
        => PutForResultAsync<FlightResponse>($"{_route}/arrivals/{id}", request, ct);

    public Task<FlightResponse> UpdateDepartureAsync(long id, UpdateDepartureRequest request, CancellationToken ct = default)
        => PutForResultAsync<FlightResponse>($"{_route}/departures/{id}", request, ct);

    public Task DeleteArrivalAsync(long id, bool deleteLinkedFlight = false, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, $"{_route}/arrivals/{id}?deleteLinkedFlight={deleteLinkedFlight}", null, ct);

    public Task DeleteDepartureAsync(long id, bool deleteLinkedFlight = false, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, $"{_route}/departures/{id}?deleteLinkedFlight={deleteLinkedFlight}", null, ct);

    public Task UnlinkArrivalAsync(long id, CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, $"{_route}/arrivals/{id}/unlink", null, ct);

    public Task UnlinkDepartureAsync(long id, CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, $"{_route}/departures/{id}/unlink", null, ct);

    public Task LinkArrivalAsync(long id, long departureId, CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, $"{_route}/arrivals/{id}/link", new LinkFlightRequest(departureId), ct);

    public Task LinkDepartureAsync(long id, long arrivalId, CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, $"{_route}/departures/{id}/link", new LinkFlightRequest(arrivalId), ct);

    public Task<IReadOnlyList<GanttItem>> GetGanttItemsAsync(string type, DateOnly fromDate, DateOnly toDate, CancellationToken ct = default)
        => GetListAsync<GanttItem>($"/api/resources/{Uri.EscapeDataString(type)}/timeline?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}", ct);

    public Task<OpsOverview> GetOpsOverviewAsync(DateOnly date, CancellationToken ct = default)
        => GetAsync($"/api/flights/ops-overview?date={date:yyyy-MM-dd}", new OpsOverview(0, 0, 0, [], [], []), ct);

    public Task<IReadOnlyList<UnassignedFlightItem>> GetUnassignedFlightsAsync(string type, DateOnly fromDate, DateOnly toDate, CancellationToken ct = default)
        => GetListAsync<UnassignedFlightItem>($"/api/resources/{Uri.EscapeDataString(type)}/unassigned?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}", ct);

    public Task<IReadOnlyList<AuditEntryDto>> GetArrivalAuditAsync(long id, CancellationToken ct = default)
        => GetListAsync<AuditEntryDto>($"{_route}/arrivals/{id}/audit", ct);

    public Task<IReadOnlyList<AuditEntryDto>> GetDepartureAuditAsync(long id, CancellationToken ct = default)
        => GetListAsync<AuditEntryDto>($"{_route}/departures/{id}/audit", ct);

    public Task<IReadOnlyList<FlightResourceResponse>> GetFlightResourcesAsync(string route, CancellationToken ct = default)
        => GetListAsync<FlightResourceResponse>(route, ct);

    public Task<FlightResourceResponse> AddFlightResourceAsync(string route, FlightResourceRequest request, CancellationToken ct = default)
        => PostForResultAsync<FlightResourceResponse>(route, request, ct);

    public Task<FlightResourceResponse> UpdateFlightResourceAsync(string route, FlightResourceRequest request, CancellationToken ct = default)
        => PutForResultAsync<FlightResourceResponse>(route, request, ct);

    public Task DeleteFlightResourceAsync(string route, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, route, null, ct);

    public Task<IReadOnlyList<FlightResourceResponse>> BulkUpdateFlightResourceStatusAsync(string routeBase, string statusCode, CancellationToken ct = default)
        => PutForListAsync<FlightResourceResponse>($"{routeBase}/status", new BulkResourceStatusRequest(statusCode), ct);

    public Task<IReadOnlyList<FlightResourceResponse>> GetCounterResourcesAsync(string route, CancellationToken ct = default)
        => GetListAsync<FlightResourceResponse>(route, ct);

    public Task<FlightResourceResponse> AddCounterResourceAsync(string route, FlightResourceRequest request, CancellationToken ct = default)
        => PostForResultAsync<FlightResourceResponse>(route, request, ct);

    public Task<FlightResourceResponse> UpdateCounterResourceAsync(string route, FlightResourceRequest request, CancellationToken ct = default)
        => PutForResultAsync<FlightResourceResponse>(route, request, ct);

    public Task DeleteCounterResourceAsync(string route, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, route, null, ct);

    public Task<IReadOnlyList<FlightResourceResponse>> BulkUpdateCounterResourceStatusAsync(string routeBase, string statusCode, CancellationToken ct = default)
        => PutForListAsync<FlightResourceResponse>($"{routeBase}/status", new BulkResourceStatusRequest(statusCode), ct);

    public async Task<int> GetResourceDefaultDurationMinutesAsync(string kind, CancellationToken ct = default)
    {
        var result = await GetAsync($"/api/flights/resource-defaults/{Uri.EscapeDataString(kind)}",
            new ResourceDefaultsResponse(ResourceDefaultsConstants.FallbackDurationMinutes), ct);
        return result.DurationMinutes;
    }

    private sealed record ResourceDefaultsResponse(int DurationMinutes);

    // No client-side caching — the API already caches this for 30s per tenant.
    public async Task<IReadOnlyDictionary<string, string>> GetFlightStatusColorsAsync(CancellationToken ct = default)
        => await GetAsync("/api/flights/status-colors", new Dictionary<string, string>(), ct);

    public async Task<string> GetResourceGridDateFormatAsync(CancellationToken ct = default)
    {
        var result = await GetAsync("/api/flights/resource-grid-date-format", new ResourceGridDateFormatResponse("dd.MM"), ct);
        return result.DateFormat;
    }

    private sealed record ResourceGridDateFormatResponse(string DateFormat);

    public Task<IReadOnlyList<OpsRoutingResponse>> GetRoutingsAsync(string route, CancellationToken ct = default)
        => GetListAsync<OpsRoutingResponse>(route, ct);

    public Task<OpsRoutingResponse> AddRoutingAsync(string route, OpsRoutingRequest request, CancellationToken ct = default)
        => PostForResultAsync<OpsRoutingResponse>(route, request, ct);

    public Task<OpsRoutingResponse> UpdateRoutingAsync(string route, OpsRoutingRequest request, CancellationToken ct = default)
        => PutForResultAsync<OpsRoutingResponse>(route, request, ct);

    public Task DeleteRoutingAsync(string route, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, route, null, ct);

    public Task<IReadOnlyList<OpsParkPositionResponse>> GetParkPositionsAsync(string route, CancellationToken ct = default)
        => GetListAsync<OpsParkPositionResponse>(route, ct);

    public Task<OpsParkPositionResponse> AddParkPositionAsync(string route, OpsParkPositionRequest request, CancellationToken ct = default)
        => PostForResultAsync<OpsParkPositionResponse>(route, request, ct);

    public Task<OpsParkPositionResponse> UpdateParkPositionAsync(string route, OpsParkPositionRequest request, CancellationToken ct = default)
        => PutForResultAsync<OpsParkPositionResponse>(route, request, ct);

    public Task DeleteParkPositionAsync(string route, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, route, null, ct);

    public Task<IReadOnlyList<CodeShareResponse>> GetCodeSharesAsync(string route, CancellationToken ct = default)
        => GetListAsync<CodeShareResponse>(route, ct);

    public Task<CodeShareResponse> AddCodeShareAsync(string route, CodeShareRequest request, CancellationToken ct = default)
        => PostForResultAsync<CodeShareResponse>(route, request, ct);

    public Task<CodeShareResponse> UpdateCodeShareAsync(string route, CodeShareRequest request, CancellationToken ct = default)
        => PutForResultAsync<CodeShareResponse>(route, request, ct);

    public Task DeleteCodeShareAsync(string route, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, route, null, ct);

    public Task<IReadOnlyList<DelayResponse>> GetDelaysAsync(string route, CancellationToken ct = default)
        => GetListAsync<DelayResponse>(route, ct);

    public Task<DelayResponse> AddDelayAsync(string route, DelayRequest request, CancellationToken ct = default)
        => PostForResultAsync<DelayResponse>(route, request, ct);

    public Task<DelayResponse> UpdateDelayAsync(string route, DelayRequest request, CancellationToken ct = default)
        => PutForResultAsync<DelayResponse>(route, request, ct);

    public Task DeleteDelayAsync(string route, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, route, null, ct);

    public Task<IReadOnlyList<MovementMessageResponse>> GetMovementMessagesAsync(string route, CancellationToken ct = default)
        => GetListAsync<MovementMessageResponse>(route, ct);

    public Task<IReadOnlyList<ServiceResponse>> GetServicesAsync(string route, CancellationToken ct = default)
        => GetListAsync<ServiceResponse>(route, ct);

    public Task<ServiceResponse> AddServiceAsync(string route, ServiceRequest request, CancellationToken ct = default)
        => PostForResultAsync<ServiceResponse>(route, request, ct);

    public Task<ServiceResponse> UpdateServiceAsync(string route, ServiceRequest request, CancellationToken ct = default)
        => PutForResultAsync<ServiceResponse>(route, request, ct);

    public Task DeleteServiceAsync(string route, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, route, null, ct);

    // direction is the plural route segment ("arrivals"/"departures"), matching every other
    // per-flight resource route in this client.
    public Task<bool> UpdateFlightStatusAsync(string direction, long id, string statusCode, CancellationToken ct = default)
        => PutForResultAsync($"/api/flights/{direction}/{id}/status", new UpdateFlightStatusRequest(statusCode), false, ct);

    public Task<bool> UpdateFlightTerminalAsync(string direction, long id, string? terminalCode, CancellationToken ct = default)
        => PutForResultAsync($"/api/flights/{direction}/{id}/terminal", new UpdateFlightTerminalRequest(terminalCode), false, ct);

    public Task<bool> UpdateFlightRegistrationAsync(string direction, long id, string? aircraftCode, CancellationToken ct = default)
        => PutForResultAsync($"/api/flights/{direction}/{id}/registration", new UpdateFlightRegistrationRequest(aircraftCode), false, ct);
}
