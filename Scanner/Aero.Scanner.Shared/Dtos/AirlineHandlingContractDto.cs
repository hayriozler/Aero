namespace Aero.Scanner.Shared.Dtos;

public sealed record AirlineHandlingContractDto(
    int Id,
    string AirlineCode,
    string HandlingCompanyCode,
    string HandlingCompanyName,
    DateTime ValidFrom,
    DateTime ValidTo,
    bool IsActive,
    string? Notes,
    IReadOnlyList<string> FlightNumbers,
    DateTime CreatedAt,
    DateTime UpdatedAt);
