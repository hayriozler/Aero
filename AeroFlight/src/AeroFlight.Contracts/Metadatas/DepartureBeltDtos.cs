namespace AeroFlight.Contracts.Metadatas;

public sealed record DepartureBeltRequest(
    string Code, string? TerminalCode, string? Description, int? Capacity);

public sealed record DepartureBeltResponse(string Code, string? TerminalCode, string? Description, int? Capacity);
