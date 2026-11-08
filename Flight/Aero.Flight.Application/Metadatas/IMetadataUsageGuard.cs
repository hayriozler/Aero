namespace Aero.Flights.Application.Metadatas;

public interface IMetadataUsageGuard<TEntity> where TEntity : class
{
    Task<string?> GetBlockReasonAsync(TEntity entity, CancellationToken ct);
}
