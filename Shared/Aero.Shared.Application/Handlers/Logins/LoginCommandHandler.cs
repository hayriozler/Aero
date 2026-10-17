using Aero.Flights.Application.Common;
using Aero.Shared.Application.Commands.Logins;
using Aero.Shared.Application.Tenancy;
using Aero.Shared.Contracts.Response;
using Aero.Shared.Infrastructure.Persistence;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Shared.Application.Handlers.Logins;

public sealed class LoginCommandHandler(
    IDataStoreContext db,
    IPasswordHasherService passwordHasher,
    IJwtTokenService jwtTokenService,
    ITenantContext tenantContext) : ICommandHandler<LoginCommand, LoginResponse?>
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