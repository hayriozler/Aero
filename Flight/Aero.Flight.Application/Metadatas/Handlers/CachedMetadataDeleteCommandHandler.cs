using Aero.Flights.Application.Metadatas.Commands;
using Aero.Shared.Application;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Application.Metadatas.Handlers;

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
