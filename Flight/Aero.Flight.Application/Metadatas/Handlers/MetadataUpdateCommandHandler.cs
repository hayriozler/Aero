using Aero.Flights.Application.Metadatas.Commands;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.Metadatas.Handlers;

public sealed class MetadataUpdateCommandHandler<TEntity, TRequest, TResponse>(
    AeroFlightDbContext db, IMetadataMap<TEntity, TRequest, TResponse> map)
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
