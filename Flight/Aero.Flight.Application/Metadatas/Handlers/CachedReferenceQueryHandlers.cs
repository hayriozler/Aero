using Aero.Shared.Application;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Application.Metadatas.Handlers;

internal static class MetadataCacheDuration
{
    public static readonly TimeSpan Lookup = TimeSpan.FromHours(12);

    public static readonly TimeSpan Crud = TimeSpan.FromSeconds(30);
}

internal static class MetadataCacheKeys
{
    public static string List<TEntity>(string tenantId) => $"refdata:{tenantId}:{typeof(TEntity).Name}:list";
    public static string Get<TEntity>(string tenantId, string id) => $"refdata:{tenantId}:{typeof(TEntity).Name}:get:{id}";
}

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
