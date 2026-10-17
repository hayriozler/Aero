using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;

namespace Aero.Flights.Application.Metadatas;

public sealed class MetadataCreateCommandHandler<TEntity, TRequest, TResponse>(
    IAeroFlightDbContext db, IMetadataMap<TEntity, TRequest, TResponse> map)
    : ICommandHandler<MetadataCreateCommand<TEntity, TRequest>, TResponse>
    where TEntity : class
{
    public async Task<TResponse> HandleAsync(MetadataCreateCommand<TEntity, TRequest> command, CancellationToken ct)
    {
        var entity = map.Create(command.Request);
        map.Apply(entity, command.Request);
        db.Add(entity);
        await db.SaveChangesAsync(ct);
        return map.ToResponse(entity);
    }
}

public sealed class MetadataUpdateCommandHandler<TEntity, TRequest, TResponse>(
    IAeroFlightDbContext db, IMetadataMap<TEntity, TRequest, TResponse> map)
    : ICommandHandler<MetadataUpdateCommand<TEntity, TRequest>, TResponse?>
    where TEntity : class
{
    public async Task<TResponse?> HandleAsync(MetadataUpdateCommand<TEntity, TRequest> command, CancellationToken ct)
    {
        var entity = await db.FindAsync<TEntity>(command.Id, ct);
        if (entity is null) return default;

        map.Apply(entity, command.Request);
        await db.SaveChangesAsync(ct);
        return map.ToResponse(entity);
    }
}

public sealed class MetadataDeleteCommandHandler<TEntity>(IAeroFlightDbContext db, IMetadataUsageGuard<TEntity>? guard = null)
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
