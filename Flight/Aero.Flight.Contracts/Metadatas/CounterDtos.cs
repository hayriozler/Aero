namespace Aero.Flights.Contracts.Metadatas;

public sealed record CounterRequest(string Code, string? TerminalCode, string? Description, int? Capacity);

public sealed record CounterResponse(string Code, string? TerminalCode, string? Description, int? Capacity);
