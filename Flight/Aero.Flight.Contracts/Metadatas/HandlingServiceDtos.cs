namespace Aero.Flights.Contracts.Metadatas;

public sealed record HandlingServiceRequest(string Code, string Unit, string? Description);

public sealed record HandlingServiceResponse(string Code, string Unit, string? Description);
