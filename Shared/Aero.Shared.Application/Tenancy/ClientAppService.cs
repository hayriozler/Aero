using Aero.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Aero.Shared.Application.Tenancy;

internal sealed class ClientAppService(
    IDbContextFactory<TenantDbContext> contextFactory, IStringProtector protector, TimeProvider clock) : IClientAppService
{
    public async Task<bool> VerifyAsync(string tenantId, string clientId, string clientSecret, CancellationToken ct)
    {
        var secret = await GetSecretAsync(tenantId, clientId, ct);
        if (secret is null)
            return false;

        var expected = Encoding.UTF8.GetBytes(secret);
        var actual = Encoding.UTF8.GetBytes(clientSecret);
        return expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    public async Task<string?> GetSecretAsync(string tenantId, string clientId, CancellationToken ct)
    {
        await using var db = await contextFactory.CreateDbContextAsync(ct);
        var clientApp = await db.ClientAppSet.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == clientId && c.TenantId == tenantId, ct);
        if (clientApp is null)
            return null;

        var now = clock.GetUtcNow();
        if (now < clientApp.BeginDateTime || (clientApp.EndDateTime is { } endDateTime && now > endDateTime))
            return null;

        return protector.Unprotect(clientApp.Secret);
    }
}
