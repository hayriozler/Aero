namespace Aero.Flights.Contracts.SeasonalFlightSchedules;

public sealed record ScheduleSeasonRequest(
    string Code,
    string? Description,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate);

public sealed record ScheduleSeasonResponse(
    string Code,
    string? Description,
    DateTimeOffset BeginDate,
    DateTimeOffset EndDate);
