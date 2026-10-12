using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AeroFlight.Application.Configuration;

public interface IAeroConfigService
{
    Task<string?> GetAsync(string configurationName, CancellationToken ct);
    Task<int> GetIntAsync(string configurationName, int fallback, CancellationToken ct);
    Task<IReadOnlyDictionary<string, string>> GetByPrefixAsync(string prefix, CancellationToken ct);
    Task SetAsync(string configurationName, string value, CancellationToken ct);
}

public sealed class AeroConfigService(IAeroFlightDbContext db, IMemoryCache cache, ITenantContext tenant) : IAeroConfigService
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
            db.Add(AeroConfiguration.Create(configurationName, value, null));
        else
            row.UpdateValue(value);

        await db.SaveChangesAsync(ct);
        cache.Remove($"aeroconfig:{tenant.TenantId}:name:{configurationName}");
    }
}
