namespace Aero.Flights.Application.Flights.Queries;

public sealed record OpsOverview(
    int ArrivalsCount,
    int DeparturesCount,
    int UnallocatedGatesCount,
    IReadOnlyList<OpsMovementTime> MovementTimes,
    IReadOnlyList<OpsResourceOccupancy> ResourceOccupancy,
    IReadOnlyList<OpsUpcomingDeparture> NextOffBlocks);

public sealed record OpsMovementTime(DateTimeOffset ScheduledTime, string Direction);

public sealed record OpsResourceOccupancy(string ResourceType, int OccupiedCount);

public sealed record OpsUpcomingDeparture(
    long DepartureFlightId,
    string FlightCode,
    string DestinationAirportCode,
    DateTimeOffset Std,
    DateTimeOffset? Etd,
    string? Gate,
    string Status);
