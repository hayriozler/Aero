namespace Aero.Shared.Application.Commands.Logins;

public sealed record ClientTokenCommand(string ClientId, string ClientSecret);