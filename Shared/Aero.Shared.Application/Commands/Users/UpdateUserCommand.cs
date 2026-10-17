namespace Aero.Shared.Application.Commands.Users;

public sealed record UpdateUserCommand(int Id, string? RealName);