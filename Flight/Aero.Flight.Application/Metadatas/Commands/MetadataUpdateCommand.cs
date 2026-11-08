namespace Aero.Flights.Application.Metadatas.Commands;

public sealed record MetadataUpdateCommand<TEntity, TRequest>(string Id, TRequest Request);