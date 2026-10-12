using AeroFlight.Application.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeroFlight.Infrastructure.Tenancy;

// Caches the decrypted tenant registry in memory.
internal sealed class TenantStore(
    IDbContextFactory<TenantDbContext> contextFactory,
    IStringProtector protector,
    TimeProvider clock,
    ILogger<TenantStore> logger) : ITenantStore
{
    private static readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(5);

    private readonly SemaphoreSlim _gate = new(1, 1);
    private Dictionary<string, TenantInfo> _cache = new(StringComparer.OrdinalIgnoreCase);
    private DateTimeOffset _loadedAt = DateTimeOffset.MinValue;

    public async Task<TenantInfo?> FindAsync(string tenantId, CancellationToken ct = default)
        => (await GetSnapshotAsync(ct)).GetValueOrDefault(tenantId);

    public async Task<IReadOnlyList<TenantInfo>> GetActiveAsync(CancellationToken ct = default)
        => (await GetSnapshotAsync(ct)).Values.Where(t => t.IsActive).ToList();

    public void Invalidate() => _loadedAt = DateTimeOffset.MinValue;

    private async Task<Dictionary<string, TenantInfo>> GetSnapshotAsync(CancellationToken ct)
    {
        if (clock.GetUtcNow() - _loadedAt < _cacheTtl)
            return _cache;

        await _gate.WaitAsync(ct);
        try
        {
            if (clock.GetUtcNow() - _loadedAt < _cacheTtl)
                return _cache;

            await using var db = await contextFactory.CreateDbContextAsync(ct);
            var tenants = await db.TenantSet.AsNoTracking().ToListAsync(ct);

            var map = new Dictionary<string, TenantInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var tenant in tenants)
            {
                try
                {
                    var connectionString = protector.Unprotect(tenant.ConnectionStringProtected);
                    map[tenant.Id] = new TenantInfo(tenant.Id, tenant.Name, connectionString, tenant.HomeAirportCode, tenant.IsActive);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to decrypt connection string for tenant {TenantId}; skipping.", tenant.Id);
                }
            }

            _cache = map;
            _loadedAt = clock.GetUtcNow();
            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }
}
