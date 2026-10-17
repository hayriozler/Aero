using Aero.Shared.Application.Tenancy;
using Aero.Shared.Domain;
using Aero.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Managers;

internal sealed class InterfaceSettingsManager(
    IDbContextFactory<TenantDbContext> contextFactory, IStringProtector protector, TimeProvider clock)
    : IInterfaceSettingsManager
{
    private static readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(5);

    private readonly SemaphoreSlim _gate = new(1, 1);
    private Dictionary<(string TenantId, string Key), string> _cache = new();
    private DateTimeOffset _loadedAt = DateTimeOffset.MinValue;

    public async Task<string?> FindAsync(string tenantId, string key, CancellationToken ct = default)
        => (await GetSnapshotAsync(ct)).GetValueOrDefault((tenantId, key));

    public async Task SetAsync(string tenantId, string key, string value, CancellationToken ct = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(ct);
        var existing = await db.TenantInterfaceSettingSet.FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Key == key, ct);
        var protectedValue = protector.Protect(value);
        if (existing is null)
            db.TenantInterfaceSettingSet.Add(TenantInterfaceSetting.Create(tenantId, key, protectedValue));
        else
            existing.UpdateValue(protectedValue);

        await db.SaveChangesAsync(ct);
        Invalidate();
    }

    public void Invalidate() => _loadedAt = DateTimeOffset.MinValue;

    private async Task<Dictionary<(string, string), string>> GetSnapshotAsync(CancellationToken ct)
    {
        if (clock.GetUtcNow() - _loadedAt < _cacheTtl)
            return _cache;

        await _gate.WaitAsync(ct);
        try
        {
            if (clock.GetUtcNow() - _loadedAt < _cacheTtl)
                return _cache;

            await using var db = await contextFactory.CreateDbContextAsync(ct);
            var settings = await db.TenantInterfaceSettingSet.AsNoTracking().ToListAsync(ct);

            var map = new Dictionary<(string, string), string>();
            foreach (var setting in settings)
                map[(setting.TenantId, setting.Key)] = protector.Unprotect(setting.Value);

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
