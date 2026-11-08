using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Application.Metadatas.Handlers;

internal sealed class CachedMetadataListQueryHandler<TEntity, TResponse>(
    AeroFlightDbContext db,
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
