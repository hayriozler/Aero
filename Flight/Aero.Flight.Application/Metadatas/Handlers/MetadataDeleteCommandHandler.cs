using Aero.Flights.Application.Metadatas.Commands;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.Metadatas.Handlers;

public sealed class MetadataDeleteCommandHandler<TEntity>(AeroFlightDbContext db, IMetadataUsageGuard<TEntity>? guard = null)
    : ICommandHandler<MetadataDeleteCommand<TEntity>, MetadataDeleteResult>
    where TEntity : class
{
    public async Task<MetadataDeleteResult> HandleAsync(MetadataDeleteCommand<TEntity> command, CancellationToken ct)
    {
        var entity = await db.FindAsync<TEntity>(command.Id, ct);
        if (entity is null) return MetadataDeleteResult.NotFound();

        if (guard is not null)
        {
            var blockReason = await guard.GetBlockReasonAsync(entity, ct);
            if (blockReason is not null)
                return MetadataDeleteResult.InUse(blockReason);
        }

        db.Remove(entity);
        await db.SaveChangesAsync(ct);
        return MetadataDeleteResult.Deleted();
    }
}