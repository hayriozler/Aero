using Aero.Flights.Domain.Common;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Shared.Application.Managers;

public sealed class AeroConfigManager(IDataStoreContext db, IMemoryCache cache, ITenantContext tenant) : IAeroConfigManager
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30);

    public Task<string?> GetAsync(string configurationName, CancellationToken ct)
    {
        var key = $"aeroconfig:{tenant.TenantId}:name:{configurationName}";
        return cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
            return await db.Query<AeroConfiguration>()
                .Where(c => c.ConfigurationName == configurationName)
                .Select(c => c.Value)
                .FirstOrDefaultAsync(ct);
        });
    }

    public async Task<int> GetIntAsync(string configurationName, int fallback, CancellationToken ct)
    {
        var value = await GetAsync(configurationName, ct);
        return int.TryParse(value, out var parsed) ? parsed : fallback;
    }

    public Task<IReadOnlyDictionary<string, string>> GetByPrefixAsync(string prefix, CancellationToken ct)
    {
        var key = $"aeroconfig:{tenant.TenantId}:prefix:{prefix}";
        return cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;

            var rows = await db.Query<AeroConfiguration>()
                .Where(c => c.ConfigurationName.StartsWith(prefix) && c.Value != null)
                .Select(c => new { c.ConfigurationName, c.Value })
                .ToListAsync(ct);

            IReadOnlyDictionary<string, string> result = rows.ToDictionary(
                r => r.ConfigurationName[prefix.Length..],
                r => r.Value!,
                StringComparer.OrdinalIgnoreCase);

            return result;
        })!;
    }

    public async Task SetAsync(string configurationName, string value, CancellationToken ct)
    {
        var row = await db.Query<AeroConfiguration>().FirstOrDefaultAsync(c => c.ConfigurationName == configurationName, ct);
        if (row is null)
            db.AddEntity(AeroConfiguration.Create(configurationName, value, null));
        else
            row.UpdateValue(value);

        await db.SaveChangesAsync(ct);
        cache.Remove($"aeroconfig:{tenant.TenantId}:name:{configurationName}");
    }
}

