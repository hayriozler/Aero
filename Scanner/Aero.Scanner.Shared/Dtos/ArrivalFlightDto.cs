using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Dtos;

public sealed record ArrivalFlightDto(
    int Id,
    string RemoteSystemId,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate,
    string Destination,
    DateTime OperationDateTime,
    ArrivalFlightStatus Status,
    string? Carousel,
    string? IntDom,
    string? Terminal,
    string? HandlingCompanyCode,
    string? HandlingCompanyName,
    // Arrival bag statistics (sourced from latest ReconciliationRecord)
    int ExpectedBagCount,
    int UnloadedBagCount,
    int RemainingOnAircraftBagCount,
    int ToBeltBagCount,
    int DeliveredBagCount,
    int TransferBagCount,
    int MissingBagCount,
    int UnknownBagCount,
    int RushBagCount);
