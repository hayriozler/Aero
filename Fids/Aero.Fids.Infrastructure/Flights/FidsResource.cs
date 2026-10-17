namespace Aero.Fids.Infrastructure.Flights;

public sealed record FidsResource(string Kind, string Code, string? Status, string? ExternalStatus, string? DisplayCode = null);
