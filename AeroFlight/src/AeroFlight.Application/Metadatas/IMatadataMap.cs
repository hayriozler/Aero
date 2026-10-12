using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public interface IMetadataReader<TEntity, TResponse>
    where TEntity : class
{
    string GetCode(TEntity entity);
    TResponse ToResponse(TEntity entity);

    // Translatable counterparts of GetCode/ToResponse — MetadataListQueryHandler/MetadataGetQueryHandler
    // use these to let EF project straight to TResponse in SQL (only the needed columns, ordered/filtered
    // in the database) instead of loading full entities and mapping in memory. GetCode/ToResponse remain
    // the ones used for Create/Update/outbox, which already have the full entity in hand.
    System.Linq.Expressions.Expression<Func<TEntity, string>> CodeSelector { get; }
    System.Linq.Expressions.Expression<Func<TEntity, TResponse>> Projection { get; }
}

public interface IMetadataMap<TEntity, TRequest, TResponse> : IMetadataReader<TEntity, TResponse>
    where TEntity : class
{
    TEntity Create(TRequest request);
    void Apply(TEntity entity, TRequest request);
}
