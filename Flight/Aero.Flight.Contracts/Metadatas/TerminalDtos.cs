namespace Aero.Flights.Contracts.Metadatas;

public sealed record TerminalRequest(string Code, string? Description);

public sealed record TerminalResponse(string Code, string? Description);
