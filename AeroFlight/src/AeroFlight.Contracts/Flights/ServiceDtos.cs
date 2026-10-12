namespace AeroFlight.Contracts.Flights;

public sealed record ServiceRequest(
    string ServiceCode,
    DateTimeOffset? BeginPlan,
    DateTimeOffset? EndPlan,
    decimal? AmountPlan,
    DateTimeOffset? BeginActual,
    DateTimeOffset? EndActual,
    decimal? AmountActual,
    string? HandlingAgentCode,
    string? Comment,
    string? PayerCompanyCode = null);

public sealed record ServiceResponse(
    long Id,
    long FlightId,
    string ServiceCode,
    DateTimeOffset? BeginPlan,
    DateTimeOffset? EndPlan,
    decimal? AmountPlan,
    DateTimeOffset? BeginActual,
    DateTimeOffset? EndActual,
    decimal? AmountActual,
    string? HandlingAgentCode,
    string? Comment,
    string? PayerCompanyCode);
