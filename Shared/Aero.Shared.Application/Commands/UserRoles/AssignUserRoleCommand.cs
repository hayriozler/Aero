namespace Aero.Shared.Application.Commands.UserRoles;

public sealed record AssignUserRoleCommand(int UserId, string RoleName);