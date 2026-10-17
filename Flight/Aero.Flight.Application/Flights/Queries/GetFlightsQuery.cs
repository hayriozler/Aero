namespace Aero.Flights.Application.Flights.Queries;

public sealed record GetFlightsQuery(
    DateOnly? FromDate,
    DateOnly? ToDate,
    IReadOnlyList<string>? Directions,
    string? AirlineCode,
    string? Search,
    IReadOnlyList<string>? TerminalCodes,
    IReadOnlyList<string>? CountryTypes,
    IReadOnlyList<string>? Statuses,
    IReadOnlyList<string>? FlightTypeCodes,
    int Page,
    int? PageSize);
