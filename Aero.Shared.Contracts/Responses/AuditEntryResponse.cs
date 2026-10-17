namespace Aero.Shared.Contracts.Responses;

public sealed record AuditEntryResponse(long Id, string Action, string ChangedBy, DateTime Timestamp, string Snapshot);