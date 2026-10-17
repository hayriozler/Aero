namespace Aero.Shared.Application.Commands.RolePermissions;

public sealed record SetRolePermissionsCommand(string RoleName, IReadOnlyList<string> PermissionCodes);
