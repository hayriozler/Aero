using Aero.Flights.Application.Metadatas.Commands;
using Aero.Shared.Application;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Application.Metadatas.Handlers;

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
