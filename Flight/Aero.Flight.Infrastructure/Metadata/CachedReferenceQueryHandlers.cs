using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.Metadatas;
using Aero.Flights.Application.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Infrastructure.Metadata;

internal static class MetadataCacheDuration
{
    // Pure lookups/statuses never change at runtime, so a long cache is safe.
    public static readonly TimeSpan Lookup = TimeSpan.FromHours(12);

    // Mutable CRUD Metadata entities can still be cached — the window itself no longer matters
    // for freshness (Create/Update/Delete evict the relevant keys immediately, see
    // CachedMetadataQueryHandlers.cs), it just bounds how long a *never-written* entry lingers.
    public static readonly TimeSpan Crud = TimeSpan.FromSeconds(30);
}

// Single place the cache key format is built, shared by the Cached*QueryHandlers below and the
// Cached*CommandHandlers in CachedMetadataCommandHandlers.cs — those evict exactly these keys
// right after a write commits, so a key typo here would silently break invalidation on one side.
internal static class MetadataCacheKeys
{
    public static string List<TEntity>(string tenantId) => $"refdata:{tenantId}:{typeof(TEntity).Name}:list";
    public static string Get<TEntity>(string tenantId, string id) => $"refdata:{tenantId}:{typeof(TEntity).Name}:get:{id}";
}

// Registered via AddCachedMetadataLookup for entities with no Create/Update/Delete handlers.
// Reads the full list itself (AsNoTracking) instead of delegating to MetadataListQueryHandler —
// these entities are never updated, so there's no reason to pay for change tracking on a list
// that's about to be cached for 12h anyway.
internal sealed class CachedMetadataListQueryHandler<TEntity, TResponse>(
    IAeroFlightDbContext db,
    IMetadataReader<TEntity, TResponse> reader,
    IMemoryCache cache,
    ITenantContext tenant)
    : IQueryHandler<MetadataListQuery<TEntity>, IReadOnlyList<TResponse>>
    where TEntity : class
{
    public Task<IReadOnlyList<TResponse>> HandleAsync(MetadataListQuery<TEntity> query, CancellationToken ct)
    {
        var key = MetadataCacheKeys.List<TEntity>(tenant.TenantId);
        return cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = MetadataCacheDuration.Lookup;
            return (IReadOnlyList<TResponse>)await db.Query<TEntity>().AsNoTracking()
                .OrderBy(reader.CodeSelector)
                .Select(reader.Projection)
                .ToListAsync(ct);
        })!;
    }
}

// Registered via AddMetadataLookup (i.e. every CRUD Metadata entity). Delegates to the tracked
// MetadataListQueryHandler rather than reading AsNoTracking itself — these entities are mutated
// elsewhere in the app, so the query stays tracked; only the cache window is short (30s) to bound
// how long an edit can appear stale instead of needing invalidate-on-write logic.
internal sealed class CachedCrudListQueryHandler<TEntity, TResponse>(
    MetadataListQueryHandler<TEntity, TResponse> inner,
    IMemoryCache cache,
    ITenantContext tenant)
    : IQueryHandler<MetadataListQuery<TEntity>, IReadOnlyList<TResponse>>
    where TEntity : class
{
    public Task<IReadOnlyList<TResponse>> HandleAsync(MetadataListQuery<TEntity> query, CancellationToken ct)
    {
        var key = MetadataCacheKeys.List<TEntity>(tenant.TenantId);
        return cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = MetadataCacheDuration.Crud;
            return inner.HandleAsync(query, ct);
        })!;
    }
}

// Shared by both AddMetadataLookup (CRUD, short duration) and AddCachedMetadataLookup (pure
// lookups, long duration) — Get always delegates to the tracked MetadataGetQueryHandler either
// way, so only the cache window differs.
internal sealed class CachedMetadataGetQueryHandler<TEntity, TResponse>(
    MetadataGetQueryHandler<TEntity, TResponse> inner,
    IMemoryCache cache,
    ITenantContext tenant,
    TimeSpan duration)
    : IQueryHandler<MetadataGetQuery<TEntity>, TResponse?>
    where TEntity : class
{
    public Task<TResponse?> HandleAsync(MetadataGetQuery<TEntity> query, CancellationToken ct)
    {
        var key = MetadataCacheKeys.Get<TEntity>(tenant.TenantId, query.Id);
        return cache.GetOrCreateAsync(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = duration;
            return inner.HandleAsync(query, ct);
        });
    }
}
