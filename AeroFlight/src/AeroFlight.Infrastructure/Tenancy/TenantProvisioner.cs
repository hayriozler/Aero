using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Infrastructure.Tenancy;

internal sealed class TenantProvisioner(
    IDbContextFactory<TenantDbContext> contextFactory,
    IStringProtector protector,
    ITenantStore store,
    TimeProvider clock) : ITenantProvisioner
{
    public async Task UpsertAsync(string id, string name, string connectionString, string homeAirportCode,
        bool isActive = true, CancellationToken ct = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(ct);

        var protectedConnectionString = protector.Protect(connectionString);
        var existing = await db.TenantSet.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (existing is null)
            db.TenantSet.Add(Tenant.Create(id, name, protectedConnectionString, homeAirportCode, isActive, clock.GetUtcNow()));
        else
            existing.Update(name, protectedConnectionString, homeAirportCode, isActive, clock.GetUtcNow());

        await db.SaveChangesAsync(ct);
        store.Invalidate();
    }
}
