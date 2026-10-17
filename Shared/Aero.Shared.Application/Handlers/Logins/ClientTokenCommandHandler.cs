using Aero.Flights.Application.Common;
using Aero.Shared.Application.Commands.Logins;
using Aero.Shared.Application.Tenancy;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Shared.Application.Handlers.Logins;

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
