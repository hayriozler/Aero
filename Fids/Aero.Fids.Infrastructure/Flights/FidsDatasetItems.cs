namespace Aero.Fids.Infrastructure.Flights;

public sealed record GateDataSetItem(
    long FlightId, string FlightType, IReadOnlyList<FidsFlightDesignator> Designators, string Status,
    DateTimeOffset ScheduledTime, string ScheduledTimeLocal, DateTimeOffset? EstimatedTime, string? EstimatedTimeLocal,
    string GateCode, string? GateStatus);

public sealed record CounterDataSetItem(
    long FlightId, string FlightType, IReadOnlyList<FidsFlightDesignator> Designators, string Status,
    DateTimeOffset ScheduledTime, string ScheduledTimeLocal, DateTimeOffset? EstimatedTime, string? EstimatedTimeLocal,
    string CounterCode, string? CounterStatus, string? CounterDisplayCode);

public sealed record BeltDataSetItem(
    long FlightId, string FlightType, IReadOnlyList<FidsFlightDesignator> Designators, string Status,
    DateTimeOffset ScheduledTime, string ScheduledTimeLocal, DateTimeOffset? EstimatedTime, string? EstimatedTimeLocal,
    string BeltCode, string? BeltStatus);

public sealed record SpotDataSetItem(
    long FlightId, string FlightType, IReadOnlyList<FidsFlightDesignator> Designators, string Status,
    DateTimeOffset ScheduledTime, string ScheduledTimeLocal, DateTimeOffset? EstimatedTime, string? EstimatedTimeLocal,
    string SpotCode, string? SpotStatus);

public sealed record ResourceUnassigned(long FlightId, string Code);

public sealed record ResourceOverviewFlight(
    long FlightId, IReadOnlyList<FidsFlightDesignator> Designators, string Status,
    DateTimeOffset ScheduledTime, string ScheduledTimeLocal, DateTimeOffset? EstimatedTime, string? EstimatedTimeLocal);

public sealed record ResourceOverviewItem(string Code, string? Status, IReadOnlyList<ResourceOverviewFlight> Flights);
