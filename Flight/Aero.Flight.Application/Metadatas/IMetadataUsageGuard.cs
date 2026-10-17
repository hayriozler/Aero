namespace Aero.Flights.Application.Metadatas;

// Optional per-entity delete guard — only entities that can actually be "in use" elsewhere
// (Airline, FlightType, AircraftType, ...) register one; MetadataDeleteCommandHandler resolves
// it as nullable (no guard registered = nothing to check, same as before this existed).
public interface IMetadataUsageGuard<TEntity> where TEntity : class
{
    // Returns a human-readable reason the entity can't be deleted, or null if it's clear to delete.
    Task<string?> GetBlockReasonAsync(TEntity entity, CancellationToken ct);
}
