namespace AeroFlight.Contracts.Metadatas;

public sealed record DelayReasonRequest(string Code, string? AlphabeticCode, string? Description, string? Comment);

public sealed record DelayReasonResponse(string Code, string? AlphabeticCode, string? Description);
