namespace Aero.Shared.Contracts.Dtos;

public sealed record AuditEntryDto(
    long Id,
    string Action,
    string Snapshot,
    DateTime Timestamp,
    string ChangedBy);