namespace Aero.Flights.Web.Services.Identity;

public sealed record RoleDto(int Id, string Name, string? Description);

public sealed record UserDto(int Id, string Username, string? RealName, bool IsActive, IReadOnlyList<string> Roles);

public sealed record PermissionDto(string Code, string? Description);
