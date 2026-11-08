namespace Aero.Flights.Application.Metadatas.Commands;

public sealed record MetadataCreateCommand<TEntity, TRequest>(TRequest Request);
