using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Identity;
using AeroFlight.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace AeroFlight.Migrations.Seeders;

internal sealed partial class ClientAppSeeder(
    IDbContextFactory<TenantDbContext> contextFactory, IStringProtector protector, ITenantContext tenantContext, ILogger<ClientAppSeeder> logger)
{
    private const string _clientId = "Aero.Fids";
    private const int _secretLength = 32;

    public async Task SeedAsync(CancellationToken ct)
    {
        await using var db = await contextFactory.CreateDbContextAsync(ct);
        var exists = await db.ClientAppSet.AnyAsync(c => c.Name == _clientId && c.TenantId == tenantContext.TenantId, ct);
        if (exists)
            return;

        var secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(_secretLength));
        var clientApp = ClientApp.Create(_clientId, tenantContext.TenantId, protector.Protect(secret), DateTimeOffset.UtcNow, null);
        db.ClientAppSet.Add(clientApp);
        await db.SaveChangesAsync(ct);
        LogClientAppCreated(logger, _clientId, tenantContext.TenantId);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "ClientApp '{ClientId}' provisioned for tenant {TenantId}.")]
    private static partial void LogClientAppCreated(ILogger logger, string clientId, string tenantId);
}
