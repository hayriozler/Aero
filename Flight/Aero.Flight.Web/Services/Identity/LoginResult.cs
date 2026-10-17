namespace Aero.Flights.Web.Services.Identity;

public sealed record LoginResult(string Token, DateTimeOffset ExpiresAt, int UserId, string Username, IReadOnlyList<string> Roles, string TimeZone, string DateFormat, string TimeFormat);
