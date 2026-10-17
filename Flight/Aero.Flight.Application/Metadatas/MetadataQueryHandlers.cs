using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace Aero.Flights.Application.Metadatas;

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

public sealed class MetadataGetQueryHandler<TEntity, TResponse>(
    AeroFlightDbContext db, IMetadataReader<TEntity, TResponse> reader)
    : IQueryHandler<MetadataGetQuery<TEntity>, TResponse?>
    where TEntity : class
{
    public Task<TResponse?> HandleAsync(MetadataGetQuery<TEntity> query, CancellationToken ct) =>
        db.Query<TEntity>()
            .Where(MetadataDataExpressions.CodeEquals(reader.CodeSelector, query.Id))
            .Select(reader.Projection)
            .FirstOrDefaultAsync(ct);
}

internal static class MetadataDataExpressions
{
    // Builds "e => e.<Code> == id" from the reader's CodeSelector, so Get can filter in SQL using
    // the same expression List uses to sort — the PK property name varies per entity (Id vs Code),
    // which is exactly why this can't just be a fixed EF.Property<string>(e, "Id") lookup.
    public static Expression<Func<TEntity, bool>> CodeEquals<TEntity>(
        Expression<Func<TEntity, string>> codeSelector, string id)
    {
        var equals = Expression.Equal(codeSelector.Body, Expression.Constant(id));
        return Expression.Lambda<Func<TEntity, bool>>(equals, codeSelector.Parameters);
    }
}
