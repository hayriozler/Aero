using Aero.Shared.Application;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Application.Metadatas.Handlers;

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
