using AeroFlight.Contracts.SeasonalFlightSchedules;

namespace AeroFlight.Web.Services.SeasonalFlightSchedules;

public interface ISeasonalFlightScheduleClient
{
    Task<IReadOnlyList<ScheduleSeasonResponse>> GetSeasonsAsync(CancellationToken ct = default);
    Task<ScheduleSeasonResponse> CreateSeasonAsync(ScheduleSeasonRequest request, CancellationToken ct = default);
    Task<ScheduleSeasonResponse> UpdateSeasonAsync(string code, ScheduleSeasonRequest request, CancellationToken ct = default);
    Task DeleteSeasonAsync(string code, CancellationToken ct = default);

    Task<IReadOnlyList<SeasonalFlightScheduleArrivalFlightResponse>> GetArrivalFlightsAsync(string? scheduleSeasonCode = null, CancellationToken ct = default);
    Task<SeasonalFlightScheduleArrivalFlightResponse> CreateArrivalFlightAsync(SeasonalFlightScheduleArrivalFlightRequest request, CancellationToken ct = default);
    Task<SeasonalFlightScheduleArrivalFlightResponse> UpdateArrivalFlightAsync(int id, SeasonalFlightScheduleArrivalFlightRequest request, CancellationToken ct = default);
    Task DeleteArrivalFlightAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<SeasonalFlightScheduleDepartureFlightResponse>> GetDepartureFlightsAsync(string? scheduleSeasonCode = null, CancellationToken ct = default);
    Task<SeasonalFlightScheduleDepartureFlightResponse> CreateDepartureFlightAsync(SeasonalFlightScheduleDepartureFlightRequest request, CancellationToken ct = default);
    Task<SeasonalFlightScheduleDepartureFlightResponse> UpdateDepartureFlightAsync(int id, SeasonalFlightScheduleDepartureFlightRequest request, CancellationToken ct = default);
    Task DeleteDepartureFlightAsync(int id, CancellationToken ct = default);
}

public sealed class SeasonalFlightScheduleClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), ISeasonalFlightScheduleClient
{
    private const string _route = "/api/seasonal-flight-schedule";

    public Task<IReadOnlyList<ScheduleSeasonResponse>> GetSeasonsAsync(CancellationToken ct = default)
        => GetListAsync<ScheduleSeasonResponse>($"{_route}/seasons", ct);

    public Task<ScheduleSeasonResponse> CreateSeasonAsync(ScheduleSeasonRequest request, CancellationToken ct = default)
        => PostForResultAsync<ScheduleSeasonResponse>($"{_route}/seasons", request, ct);

    public Task<ScheduleSeasonResponse> UpdateSeasonAsync(string code, ScheduleSeasonRequest request, CancellationToken ct = default)
        => PutForResultAsync<ScheduleSeasonResponse>($"{_route}/seasons/{code}", request, ct);

    public Task DeleteSeasonAsync(string code, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, $"{_route}/seasons/{code}", null, ct);

    public Task<IReadOnlyList<SeasonalFlightScheduleArrivalFlightResponse>> GetArrivalFlightsAsync(string? scheduleSeasonCode = null, CancellationToken ct = default)
    {
        var uri = $"{_route}/arrivals";
        if (!string.IsNullOrWhiteSpace(scheduleSeasonCode))
            uri += $"?scheduleSeasonCode={Uri.EscapeDataString(scheduleSeasonCode)}";

        return GetListAsync<SeasonalFlightScheduleArrivalFlightResponse>(uri, ct);
    }

    public Task<SeasonalFlightScheduleArrivalFlightResponse> CreateArrivalFlightAsync(SeasonalFlightScheduleArrivalFlightRequest request, CancellationToken ct = default)
        => PostForResultAsync<SeasonalFlightScheduleArrivalFlightResponse>($"{_route}/arrivals", request, ct);

    public Task<SeasonalFlightScheduleArrivalFlightResponse> UpdateArrivalFlightAsync(int id, SeasonalFlightScheduleArrivalFlightRequest request, CancellationToken ct = default)
        => PutForResultAsync<SeasonalFlightScheduleArrivalFlightResponse>($"{_route}/arrivals/{id}", request, ct);

    public Task DeleteArrivalFlightAsync(int id, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, $"{_route}/arrivals/{id}", null, ct);

    public Task<IReadOnlyList<SeasonalFlightScheduleDepartureFlightResponse>> GetDepartureFlightsAsync(string? scheduleSeasonCode = null, CancellationToken ct = default)
    {
        var uri = $"{_route}/departures";
        if (!string.IsNullOrWhiteSpace(scheduleSeasonCode))
            uri += $"?scheduleSeasonCode={Uri.EscapeDataString(scheduleSeasonCode)}";

        return GetListAsync<SeasonalFlightScheduleDepartureFlightResponse>(uri, ct);
    }

    public Task<SeasonalFlightScheduleDepartureFlightResponse> CreateDepartureFlightAsync(SeasonalFlightScheduleDepartureFlightRequest request, CancellationToken ct = default)
        => PostForResultAsync<SeasonalFlightScheduleDepartureFlightResponse>($"{_route}/departures", request, ct);

    public Task<SeasonalFlightScheduleDepartureFlightResponse> UpdateDepartureFlightAsync(int id, SeasonalFlightScheduleDepartureFlightRequest request, CancellationToken ct = default)
        => PutForResultAsync<SeasonalFlightScheduleDepartureFlightResponse>($"{_route}/departures/{id}", request, ct);

    public Task DeleteDepartureFlightAsync(int id, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, $"{_route}/departures/{id}", null, ct);
}
