namespace Aero.Flights.Contracts.Metadatas;

public sealed record GateStatusResponse(string Code, string? Description, bool IsDefault, bool IsOpenValue, bool IsCloseValue);
