namespace Aero.Flights.Contracts.Metadatas;

public sealed record GateRequest(
    string Code, string? GateTypeCode, string? TerminalCode, string? Description,
    int? Capacity, bool HasLoadingBridge, bool HasPca);

public sealed record GateResponse(
    string Code, string? GateTypeCode, string? TerminalCode, string? Description,
    int? Capacity, bool HasLoadingBridge, bool HasPca);
