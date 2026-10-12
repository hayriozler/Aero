namespace AeroFlight.Domain.Common;

public abstract class AuditLogBase
{
    public long Id { get; set; }
    public string PrimaryKey { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Snapshot { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string ChangedBy { get; set; } = string.Empty;

    // Set only for audit logs of FlightResourceBase-derived entities (Gate/Belt/Counter allocations).
    // Lets a flight's full audit trail find a resource's history by FlightId directly, without going
    // through the resource's own (possibly since-deleted) row in its live table.
    public long? FlightId { get; set; }
}
