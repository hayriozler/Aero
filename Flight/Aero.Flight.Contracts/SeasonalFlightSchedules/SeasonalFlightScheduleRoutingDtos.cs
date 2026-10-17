namespace Aero.Flights.Contracts.SeasonalFlightSchedules;

public sealed record SeasonalFlightScheduleRoutingRequest(
    string AirportCode,
    int LegNumber,
    string DirectionCode,
    DateTimeOffset? Sta,
    DateTimeOffset? Std);

public sealed record SeasonalFlightScheduleRoutingResponse(
    int Id,
    string AirportCode,
    int LegNumber,
    string DirectionCode,
    DateTimeOffset? Sta,
    DateTimeOffset? Std);
