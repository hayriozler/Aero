using AeroFlight.Application.Common;
using AeroFlight.Application.Configuration;
using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Tenancy;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Auth;

public record LoginCommand(string Username, string Password);

public record LoginResponse(string Token, DateTimeOffset ExpiresAt, int UserId, string Username, IReadOnlyList<string> Roles, string TimeZone, string DateFormat, string TimeFormat);

// Client-credentials login for a service principal (see ClientApp) — never a human, so it gets its
// own token-issuance method (IJwtTokenService.GenerateServiceToken) instead of the human login path.
// TimeZone is still resolved — AeroFids needs it for TenantLocalTime.Format regardless of who's calling.
public record ClientTokenCommand(string ClientId, string ClientSecret);

public record ClientTokenResponse(string Token, DateTimeOffset ExpiresAt, string TimeZone);

public sealed class ClientTokenCommandValidator : AbstractValidator<ClientTokenCommand>
{
    public ClientTokenCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.ClientSecret).NotEmpty();
    }
}

public sealed class ClientTokenCommandHandler(
    IClientAppService clientApps, IJwtTokenService jwtTokenService, ITenantContext tenantContext, IAeroConfigService aeroConfig)
    : ICommandHandler<ClientTokenCommand, ClientTokenResponse?>
{
    private static readonly string[] _roles = ["FidsService"];

    public async Task<ClientTokenResponse?> HandleAsync(ClientTokenCommand command, CancellationToken ct)
    {
        var verified = await clientApps.VerifyAsync(tenantContext.TenantId, command.ClientId, command.ClientSecret, ct);
        if (!verified)
            return null;

        var (token, expiresAt) = jwtTokenService.GenerateServiceToken(command.ClientId, tenantContext.TenantId, _roles);

        var displaySettings = await aeroConfig.GetByPrefixAsync("Display.", ct);
        var timeZone = displaySettings.GetValueOrDefault("TimeZone") ?? "UTC";

        return new ClientTokenResponse(token, expiresAt, timeZone);
    }
}

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class LoginCommandHandler(
    IAeroFlightDbContext db,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    ITenantContext tenantContext,
    IAeroConfigService aeroConfig) : ICommandHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> HandleAsync(LoginCommand command, CancellationToken ct)
    {
        var user = await db.UserSet
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == command.Username && u.IsActive, ct);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, command.Password))
            return null;

        var roles = user.UserRoles
            .Where(ur => ur.Role is not null)
            .Select(ur => ur.Role!.Name)
            .ToList();

        var (token, expiresAt) = jwtTokenService.GenerateToken(user.Id, user.Username, tenantContext.TenantId, roles);

        var displaySettings = await aeroConfig.GetByPrefixAsync("Display.", ct);
        var timeZone = displaySettings.GetValueOrDefault("TimeZone") ?? "UTC";
        var dateFormat = displaySettings.GetValueOrDefault("DateFormat") ?? "dd/MM/yyyy";
        var timeFormat = displaySettings.GetValueOrDefault("TimeFormat") ?? "HH:mm";

        return new LoginResponse(token, expiresAt, user.Id, user.Username, roles, timeZone, dateFormat, timeFormat);
    }
}
