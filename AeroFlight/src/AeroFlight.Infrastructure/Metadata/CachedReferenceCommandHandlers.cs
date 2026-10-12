using AeroFlight.Application.Common;
using AeroFlight.Application.Metadatas;
using AeroFlight.Application.Tenancy;
using Microsoft.Extensions.Caching.Memory;

namespace AeroFlight.Infrastructure.Metadata;

// Metadata CRUD list/get results are cached (see CachedMetadataQueryHandlers.cs), so a write
// needs to evict the entries it just invalidated — otherwise the change is invisible until the
// cache window expires. These wrappers run the tracked command handler first, then evict.
internal sealed class CachedMetadataCreateCommandHandler<TEntity, TRequest, TResponse>(
    MetadataCreateCommandHandler<TEntity, TRequest, TResponse> inner,
    IMemoryCache cache,
    ITenantContext tenant)
    : ICommandHandler<MetadataCreateCommand<TEntity, TRequest>, TResponse>
    where TEntity : class
{
    public async Task<TResponse> HandleAsync(MetadataCreateCommand<TEntity, TRequest> command, CancellationToken ct)
    {
        var result = await inner.HandleAsync(command, ct);
        cache.Remove(MetadataCacheKeys.List<TEntity>(tenant.TenantId));
        return result;
    }
}

internal sealed class CachedMetadataUpdateCommandHandler<TEntity, TRequest, TResponse>(
    MetadataUpdateCommandHandler<TEntity, TRequest, TResponse> inner,
    IMemoryCache cache,
    ITenantContext tenant)
    : ICommandHandler<MetadataUpdateCommand<TEntity, TRequest>, TResponse?>
    where TEntity : class
{
    public async Task<TResponse?> HandleAsync(MetadataUpdateCommand<TEntity, TRequest> command, CancellationToken ct)
    {
        var result = await inner.HandleAsync(command, ct);
        cache.Remove(MetadataCacheKeys.List<TEntity>(tenant.TenantId));
        cache.Remove(MetadataCacheKeys.Get<TEntity>(tenant.TenantId, command.Id));
        return result;
    }
}

internal sealed class CachedMetadataDeleteCommandHandler<TEntity>(
    MetadataDeleteCommandHandler<TEntity> inner,
    IMemoryCache cache,
    ITenantContext tenant)
    : ICommandHandler<MetadataDeleteCommand<TEntity>, MetadataDeleteResult>
    where TEntity : class
{
    public async Task<MetadataDeleteResult> HandleAsync(MetadataDeleteCommand<TEntity> command, CancellationToken ct)
    {
        var result = await inner.HandleAsync(command, ct);
        if (result.Outcome == MetadataDeleteOutcome.Deleted)
        {
            cache.Remove(MetadataCacheKeys.List<TEntity>(tenant.TenantId));
            cache.Remove(MetadataCacheKeys.Get<TEntity>(tenant.TenantId, command.Id));
        }
        return result;
    }
}
