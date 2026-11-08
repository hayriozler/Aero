namespace Aero.Flights.Application.Metadatas;

public enum MetadataDeleteOutcome
{
    Deleted,
    NotFound,
    InUse
}

public sealed record MetadataDeleteResult(MetadataDeleteOutcome Outcome, string? Reason = null)
{
    public static MetadataDeleteResult Deleted() => new(MetadataDeleteOutcome.Deleted);
    public static MetadataDeleteResult NotFound() => new(MetadataDeleteOutcome.NotFound);
    public static MetadataDeleteResult InUse(string reason) => new(MetadataDeleteOutcome.InUse, reason);
}
