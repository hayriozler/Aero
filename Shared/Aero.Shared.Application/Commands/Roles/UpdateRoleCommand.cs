namespace Aero.Shared.Application.Commands.Roles;

public sealed record UpdateRoleCommand(int Id, string Name, string? Description);