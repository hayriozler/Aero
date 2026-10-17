namespace Aero.Flights.Application.Metadatas;

public interface IMetadataOutboxMapper
{
    Type EntityType { get; }
    string EntityName { get; }
    string GetCode(object entity);
    object ToResponse(object entity);
}

public sealed class MetadataOutboxMapper<TEntity, TRequest, TResponse>(
    IMetadataMap<TEntity, TRequest, TResponse> map) : IMetadataOutboxMapper
    where TEntity : class
{
    public Type EntityType => typeof(TEntity);
    public string EntityName => typeof(TEntity).Name;
    public string GetCode(object entity) => map.GetCode((TEntity)entity);
    public object ToResponse(object entity) => map.ToResponse((TEntity)entity)!;
}
