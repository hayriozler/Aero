namespace Aero.Shared.Contracts.Responses;

public sealed record LoginResponse(string Token, DateTimeOffset ExpiresAt, int UserId, string Username, IReadOnlyList<string> Roles, string TimeZone, string DateFormat, string TimeFormat);