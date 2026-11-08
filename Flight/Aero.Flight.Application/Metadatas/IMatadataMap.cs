using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public interface IMetadataReader<TEntity, TResponse>
    where TEntity : class
{
    string GetCode(TEntity entity);
    TResponse ToResponse(TEntity entity);

    Expression<Func<TEntity, string>> CodeSelector { get; }
    Expression<Func<TEntity, TResponse>> Projection { get; }
}

public interface IMetadataMap<TEntity, TRequest, TResponse> : IMetadataReader<TEntity, TResponse>
    where TEntity : class
{
    TEntity Create(TRequest request);
    void Apply(TEntity entity, TRequest request);
}
