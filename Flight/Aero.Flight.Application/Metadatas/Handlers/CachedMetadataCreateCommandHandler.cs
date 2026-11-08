using Aero.Flights.Application.Metadatas.Commands;
using Aero.Shared.Application;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Application.Metadatas.Handlers;

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
