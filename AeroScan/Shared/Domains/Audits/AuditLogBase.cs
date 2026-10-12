namespace Domain.Audits;

public abstract class AuditLogBase
{
    public long Id { get; set; }
    public string PrimaryKey { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Snapshot { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
