using Aero.Shared.Application.Commands.Logins;
using Aero.Shared.Application.Managers;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Domain;
using Aero.Shared.Infrastructure.Persistence;
using Aero.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Logins;

public sealed class LoginCommandHandler(
    IDataStoreContext db,
    IPasswordHasherService passwordHasher,
    IJwtTokenService jwtTokenService,
    ITenantContext tenantContext,
    IAeroConfigManager aeroConfig) : ICommandHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> HandleAsync(LoginCommand command, CancellationToken ct)
    {
        var user = await db.UserSet
            .Include(u => u.Roles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == command.Username && u.IsActive, ct);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, command.Password))
            return null;

        var roles = user.Roles
            .Where(ur => ur.Role is not null)
            .Select(ur => ur.Role!.Id)
            .ToList();

        var (token, expiresAt) = jwtTokenService.GenerateToken(new JwtOptions
        {
            User = user,
            TenantId = tenantContext.TenantId
        });

        var displaySettings = await aeroConfig.GetByPrefixAsync("Display.", ct);
        var timeZone = displaySettings.GetValueOrDefault("TimeZone") ?? "UTC";
        var dateFormat = displaySettings.GetValueOrDefault("DateFormat") ?? "dd/MM/yyyy";
        var timeFormat = displaySettings.GetValueOrDefault("TimeFormat") ?? "HH:mm";

        return new LoginResponse(token, expiresAt, user.Id, user.Username, roles, timeZone, dateFormat, timeFormat);
    }
}