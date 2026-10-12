namespace Aero.Scanner.Shared.Requests;

public sealed record CreateAirlineHandlingContractRequest(
    string AirlineCode,
    string HandlingCompanyCode,
    DateTime ValidFrom,
    DateTime ValidTo,
    string? Notes,
    IReadOnlyList<string> FlightNumbers);

public sealed record UpdateAirlineHandlingContractRequest(
    string AirlineCode,
    string HandlingCompanyCode,
    DateTime ValidFrom,
    DateTime ValidTo,
    string? Notes,
    IReadOnlyList<string> FlightNumbers);
