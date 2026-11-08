using Aero.Flights.Application.Metadatas.Queries;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Metadatas.Handlers;

public sealed class MetadataListQueryHandler<TEntity, TResponse>(
    AeroFlightDbContext db, IMetadataReader<TEntity, TResponse> reader)
    : IQueryHandler<MetadataListQuery<TEntity>, IReadOnlyList<TResponse>>
    where TEntity : class
{
    public async Task<IReadOnlyList<TResponse>> HandleAsync(MetadataListQuery<TEntity> query, CancellationToken ct) =>
        await db.Query<TEntity>()
            .OrderBy(reader.CodeSelector)
            .Select(reader.Projection)
            .ToListAsync(ct);
}
