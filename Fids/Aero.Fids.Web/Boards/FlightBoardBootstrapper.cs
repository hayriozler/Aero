using Aero.Fids.Infrastructure.Flights;
using Aero.Shared.Application.Tenancy;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace Aero.Fids.Web.Boards;

internal sealed class FlightBoardBootstrapper(
    HttpClient http, IFlightBoardStore store, IStatusMappingRegistry statusMappings,
    IClientAppService clientApps, TimeProvider clock, ILogger<FlightBoardBootstrapper> logger)
    : IFlightBoardBootstrapper
{
    private const string _clientId = "Aero.Fids";
    private static readonly TimeSpan _tokenRefreshMargin = TimeSpan.FromMinutes(2);
    private static readonly string[] _wantedStatuses = ["SKD", "XLD"];
    private static readonly string[] _wantedFlightTypeCodes = ["J"];
    private const int _pageSize = 1000;

    private readonly ConcurrentDictionary<(string TenantId, string FlightType), Task> _bootstraps = new();
    private readonly ConcurrentDictionary<string, Task<TenantSession?>> _sessions = new(StringComparer.OrdinalIgnoreCase);

    public Task EnsureBootstrappedAsync(string tenantId, string flightType, CancellationToken ct) =>
        _bootstraps.GetOrAdd((tenantId, flightType), key => BootstrapAsync(key.TenantId, key.FlightType, ct));

    private async Task BootstrapAsync(string tenantId, string flightType, CancellationToken ct)
    {
        var session = await GetSessionAsync(tenantId, ct);
        if (session is null)
            return;

        try
        {
            var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
            var statusMapper = await statusMappings.GetMapperAsync(tenantId, ct);
            var count = 0;
            var page = 1;

            while (true)
            {
                var url = $"/{tenantId}/api/flights?fromDate={today:yyyy-MM-dd}&toDate={today:yyyy-MM-dd}" +
                    $"&direction={flightType}&{QueryString("statuses", _wantedStatuses)}&{QueryString("flightType", _wantedFlightTypeCodes)}" +
                    $"&page={page}&pageSize={_pageSize}";

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
                using var response = await http.SendAsync(request, ct);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(ct);
                    logger.LogError(
                        "GET {Url} returned {StatusCode}. WWW-Authenticate: {WwwAuthenticate}. Body: {Body}",
                        url, (int)response.StatusCode, response.Headers.WwwAuthenticate, body);
                    response.EnsureSuccessStatusCode();
                }

                var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<List<MergedFlightRow>>>(cancellationToken: ct);
                var rows = envelope?.Data ?? [];
                foreach (var row in rows)
                {
                    var flight = ToFidsFlight(row, flightType, session.TimeZone, statusMapper);
                    if (flight is not null)
                        store.Apply(tenantId, flight);
                }
                count += rows.Count;

                var pagination = envelope?.Pagination;
                if (pagination is null || page * pagination.PageSize >= pagination.TotalCount || rows.Count == 0)
                    break;
                page++;
            }

            logger.LogInformation("Bootstrapped {Count} {FlightType} flights for tenant {TenantId}.", count, flightType, tenantId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to bootstrap {FlightType} board for tenant {TenantId}; board starts empty until RabbitMQ deltas arrive.", flightType, tenantId);
        }
    }

    private async Task<TenantSession?> GetSessionAsync(string tenantId, CancellationToken ct)
    {
        while (true)
        {
            var task = _sessions.GetOrAdd(tenantId, id => LoginAsync(id, ct));
            var session = await task;
            if (session is null)
                return null;
            if (session.ExpiresAt - _tokenRefreshMargin > clock.GetUtcNow())
                return session;

            _sessions.TryRemove(new KeyValuePair<string, Task<TenantSession?>>(tenantId, task));
        }
    }

    private async Task<TenantSession?> LoginAsync(string tenantId, CancellationToken ct)
    {
        var secret = await clientApps.GetSecretAsync(tenantId, _clientId, ct);
        if (secret is null)
        {
            logger.LogWarning("No '{ClientId}' ClientApp provisioned for tenant {TenantId}; board starts empty until RabbitMQ deltas arrive.", _clientId, tenantId);
            return null;
        }

        try
        {
            using var response = await http.PostAsJsonAsync(
                $"/{tenantId}/api/auth/client-token", new { ClientId = _clientId, ClientSecret = secret }, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                logger.LogError(
                    "client-token request for tenant {TenantId} returned {StatusCode}. Body: {Body}",
                    tenantId, (int)response.StatusCode, body);
                response.EnsureSuccessStatusCode();
            }

            var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<ClientTokenResponse>>(cancellationToken: ct);
            if (envelope?.Data is not { } token)
            {
                logger.LogError("Client-token request for tenant {TenantId} returned no data.", tenantId);
                return null;
            }

            return new TenantSession(token.Token, token.ExpiresAt, token.TimeZone);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to obtain a client-credentials token for tenant {TenantId}; board starts empty until RabbitMQ deltas arrive.", tenantId);
            return null;
        }
    }

    private static FidsFlight? ToFidsFlight(MergedFlightRow row, string flightType, string tenantTimeZone, StatusMapper statusMapper)
    {
        var isArrival = flightType == FlightTypes.Arrival;
        var id = isArrival ? row.ArrivalId : row.DepartureId;
        if (id is null)
            return null;

        var airlineCode = isArrival ? row.ArrivalAirlineCode : row.DepartureAirlineCode;
        var flightNumber = isArrival ? row.ArrivalFlightNumber : row.DepartureFlightNumber;
        if (airlineCode is null || flightNumber is null)
            return null;

        var status = isArrival ? row.ArrivalStatus : row.DepartureStatus;
        var scheduledTime = isArrival ? row.Sta : row.Std;
        var estimatedTime = isArrival ? row.Eta : row.Etd;
        var terminalCode = isArrival ? row.ArrivalTerminalCode : row.DepartureTerminalCode;
        var routeAirportCode = isArrival ? row.OriginAirportCode : row.DestinationAirportCode;

        return new FidsFlight(
            id.Value,
            flightType,
            [new FidsFlightDesignator(airlineCode, flightNumber)],
            row.FlightDate,
            statusMapper.MapFlightStatus(status, flightType),
            scheduledTime ?? DateTimeOffset.UtcNow,
            TenantLocalTime.Format(scheduledTime ?? DateTimeOffset.UtcNow, tenantTimeZone),
            estimatedTime,
            TenantLocalTime.FormatOrNull(estimatedTime, tenantTimeZone),
            terminalCode,
            routeAirportCode ?? "",
            [
                .. Split(isArrival ? row.ArrivalGates : row.DepartureGates).Select(code => new FidsResource(ResourceKinds.Gate, code, null, null)),
                .. Split(isArrival ? row.ArrivalBaggageBelts : null).Select(code => new FidsResource(ResourceKinds.Belt, code, null, null)),
                .. Split(!isArrival ? row.DepartureCounters : null).Select(code => new FidsResource(ResourceKinds.Counter, code, null, null)),
                .. Split(isArrival ? row.ArrivalParkPositions : row.DepartureParkPositions).Select(code => new FidsResource(ResourceKinds.Spot, code, null, null)),
            ]);
    }

    private static string QueryString(string key, IReadOnlyList<string> values) =>
        string.Join('&', values.Select(v => $"{key}={Uri.EscapeDataString(v)}"));

    private static string[] Split(string? value) => (value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private sealed record TenantSession(string Token, DateTimeOffset ExpiresAt, string TimeZone);

    private sealed record ApiEnvelope<T>(T? Data, ApiPagination? Pagination);

    private sealed record ApiPagination(int Page, int PageSize, int TotalCount, int TotalPages);
}
