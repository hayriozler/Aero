namespace Aero.Fids.Infrastructure.Flights;

public sealed record FidsFlight(
    long FlightId,
    string FlightType,
    IReadOnlyList<FidsFlightDesignator> Designators,
    DateOnly FlightDate,
    string Status,
    DateTimeOffset ScheduledTime,
    string ScheduledTimeLocal,
    DateTimeOffset? EstimatedTime,
    string? EstimatedTimeLocal,
    string? TerminalCode,
    string RouteAirportCode,
    IReadOnlyList<FidsResource> Resources);
