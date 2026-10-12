using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Dtos;

public sealed record DepartureFlightDto(
    int Id,
    string RemoteSystemId,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate,
    string Destination,
    DateTime OperationDateTime,
    DepartureFlightStatus Status,
    string? Terminal,
    string? IntDom,
    string? Chute,
    string? CheckIn,
    string? CheckInStatus,
    string? Gate,
    string? GateStatus,
    string? HandlingCompanyCode,
    string? HandlingCompanyName,
    // Bag counts
    int ExpectedBagCount,
    int LoadedBagCount,
    int OffloadedCount,
    int ToBeOffloadedCount,
    int WaitingToLoadBagCount,
    int MissingBagCount,
    int ReconciledBagCount,
    int ForceLoadedBagCount,
    int OnwardBagCount,
    int TransferLoadedBagCount,
    int TransferMissingBagCount,
    int NotBoardedPassengerBagCount,
    int RushBagCount,
    int PriorityBagCount);
