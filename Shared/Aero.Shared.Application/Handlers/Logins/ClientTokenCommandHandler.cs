using Aero.Shared.Application.Commands.Logins;
using Aero.Shared.Application.Managers;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Shared.Application.Handlers.Logins;

public sealed class ClientTokenCommandHandler(
    IClientAppManager clientApps, IJwtTokenService jwtTokenService, ITenantContext tenantContext, IAeroConfigManager aeroConfig)
    : ICommandHandler<ClientTokenCommand, ClientTokenResponse?>
{
    public async Task<ClientTokenResponse?> HandleAsync(ClientTokenCommand command, CancellationToken ct)
    {
        var verified = await clientApps.VerifyAsync(tenantContext.TenantId, command.ClientId, command.ClientSecret, ct);
        if (!verified)
            return null;

        var (token, expiresAt) = jwtTokenService.GenerateToken(new Domain.JwtOptions
        {

        });

        var displaySettings = await aeroConfig.GetByPrefixAsync("Display.", ct);
        var timeZone = displaySettings.GetValueOrDefault("TimeZone") ?? "UTC";

        return new ClientTokenResponse(token, expiresAt, timeZone);
    }
}
