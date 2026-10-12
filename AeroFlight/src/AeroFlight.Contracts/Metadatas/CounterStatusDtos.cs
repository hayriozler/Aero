namespace AeroFlight.Contracts.Metadatas;

public sealed record CounterStatusResponse(string Code, string? Description, bool IsDefault, bool IsOpenValue, bool IsCloseValue);
