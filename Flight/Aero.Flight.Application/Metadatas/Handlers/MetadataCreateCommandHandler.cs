using Aero.Flights.Application.Metadatas.Commands;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;

namespace Aero.Flights.Application.Metadatas.Handlers;

public sealed class MetadataCreateCommandHandler<TEntity, TRequest, TResponse>(
    AeroFlightDbContext db, IMetadataMap<TEntity, TRequest, TResponse> map)
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
