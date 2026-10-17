namespace Aero.Shared.Contracts.Responses;

public sealed record ApiError(string Code, string Message, string? Field);
