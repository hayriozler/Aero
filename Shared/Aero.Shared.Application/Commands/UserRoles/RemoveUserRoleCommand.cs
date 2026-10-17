namespace Aero.Shared.Application.Commands.UserRoles;

public sealed record RemoveUserRoleCommand(int UserId, string RoleName);