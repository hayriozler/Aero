namespace AeroFlight.Contracts.Metadatas;

public sealed record ArrivalBeltRequest(
    string Code, string? TerminalCode, string? Description, int? Capacity);

public sealed record ArrivalBeltResponse(string Code, string? TerminalCode, string? Description, int? Capacity);
