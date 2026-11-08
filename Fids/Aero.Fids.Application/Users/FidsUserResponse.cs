namespace Aero.Fids.Application.Users;

public sealed record FidsUserResponse(int Id, string Username, IReadOnlyList<string> Roles, bool IsActive);
