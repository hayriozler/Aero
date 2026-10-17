namespace Aero.Shared.Application.Commands.RolePermissions;

public sealed record SetRolePermissionsCommand(int RoleId, IReadOnlyList<string> PermissionCodes);
