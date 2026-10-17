namespace Aero.Shared.Contracts.Responses;

public sealed record ClientTokenResponse(string Token, DateTimeOffset ExpiresAt, string TimeZone);
