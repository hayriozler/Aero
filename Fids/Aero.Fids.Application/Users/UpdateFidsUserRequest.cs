namespace Aero.Fids.Application.Users;

public sealed record UpdateFidsUserRequest(string Role, bool IsActive, string? NewPassword);
