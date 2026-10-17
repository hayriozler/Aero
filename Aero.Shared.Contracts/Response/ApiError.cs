namespace Aero.Shared.Contracts.Response;

public sealed record ApiError(string Code, string Message, string? Field);
public sealed record ClientTokenResponse(string Token, DateTimeOffset ExpiresAt, string TimeZone);