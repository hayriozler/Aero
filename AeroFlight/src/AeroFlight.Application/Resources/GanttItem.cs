namespace AeroFlight.Application.Resources;

public sealed record GanttItem(
    long Id,
    long FlightId,
    string ResourceType,
    string ResourceCode,
    DateTimeOffset Start,
    DateTimeOffset End,
    string FlightDisplay,
    string Direction);
