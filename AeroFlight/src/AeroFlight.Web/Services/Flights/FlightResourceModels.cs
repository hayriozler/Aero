namespace AeroFlight.Web.Services.Flights;

public sealed record FlightResourceRequest(
    string? Code,
    DateTimeOffset? BeginPlan,
    DateTimeOffset? EndPlan,
    string? StatusCode,
    DateTimeOffset? BeginActual = null,
    DateTimeOffset? EndActual = null,
    string? PayerCompanyCode = null,
    bool IsComplimentary = false,
    string? CheckinClassCode = null);

public sealed record FlightResourceCodeOption(string Code, string Label, bool IsDefault = false, bool IsOpenValue = false, bool IsCloseValue = false);

public sealed record FlightResourceResponse(
    long Id,
    long FlightId,
    string? Code,
    string? LastCode,
    DateTimeOffset? BeginPlan,
    DateTimeOffset? EndPlan,
    DateTimeOffset? BeginActual,
    DateTimeOffset? EndActual,
    string? StatusCode,
    string? PayerCompanyCode,
    bool IsComplimentary,
    string? CheckinClassCode = null);

public sealed record BulkResourceStatusRequest(string StatusCode);
