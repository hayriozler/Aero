namespace Aero.Flights.Contracts.Flights;

public sealed record OpsParkPositionRequest(
    string? Code,
    DateTimeOffset BeginPlan,
    DateTimeOffset EndPlan,
    DateTimeOffset? BeginActual = null,
    DateTimeOffset? EndActual = null,
    string? PayerCompanyCode = null);

public sealed record OpsParkPositionResponse(
    long Id,
    long? ArrivalFlightId,
    long? DepartureFlightId,
    string? Code,
    string? LastCode,
    DateTimeOffset BeginPlan,
    DateTimeOffset EndPlan,
    DateTimeOffset? BeginActual,
    DateTimeOffset? EndActual,
    string? PayerCompanyCode);
