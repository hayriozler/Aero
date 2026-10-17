namespace Aero.Shared.Application.Commands.Users;

public sealed record CreateUserCommand(string Username, string Password, string? RealName);
