using Aero.Flights.Application.Metadatas.Queries;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas.Handlers;

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
    public static Expression<Func<TEntity, bool>> CodeEquals<TEntity>(
        Expression<Func<TEntity, string>> codeSelector, string id)
    {
        var equals = Expression.Equal(codeSelector.Body, Expression.Constant(id));
        return Expression.Lambda<Func<TEntity, bool>>(equals, codeSelector.Parameters);
    }
}