namespace Aero.Flights.Contracts.Metadatas;

public sealed record AuditEntryResponse(long Id, string Action, string ChangedBy, DateTime Timestamp, string Snapshot);
