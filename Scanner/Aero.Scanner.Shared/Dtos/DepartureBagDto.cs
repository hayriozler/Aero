
using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Dtos;

public sealed record DepartureBagDto(
    long Id,
    int FlightPassengerId,
    string TagNumber,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate,
    string Destination,
    string? SecurityNumber,
    string? SequenceNumber,
    string PassengerName,
    DepartureBaggageStatus DepartureBaggageStatus,
    string? WeightKg,
    BaggageClass Class,
    bool IsDeleted
    );
