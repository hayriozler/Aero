namespace Aero.Fids.Application.Users;

public sealed record CreateFidsUserRequest(string Username, string Password, string Role);
