
using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Dtos;

public sealed record DashboardSummaryDto(
    int ActiveFlights,
    int TotalBagsScannedToday,
    int ReconciliationErrors,
    int ThroughputPerMinute,
    IReadOnlyList<FlightReconciliationRowDto> FlightReconciliations);

public sealed record FlightReconciliationRowDto(
    // Departure Flight
    string FlightNo,
    string ScheduledDeparture,
    string? EstimatedDeparture,
    string? Gate,
    string? CheckIn,
    string? Terminal,
    DepartureFlightStatus FlightStatus,
    // Bag statistics
    int ExpectedBagCount,
    int LoadedBagCount,
    int MissingBagCount,
    int OffloadedCount,
    int RushBagCount,
    int ReconciliationPercent,
    bool HasMismatch,
    // Arrival / Destination
    string NextAirport,
    string DestinationAirport);
