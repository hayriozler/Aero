namespace Aero.Flights.Application.Metadatas.Queries;

public sealed record MetadataGetQuery<TEntity>(string Id);
public sealed record MetadataListQuery<TEntity>;