namespace AeroFlight.Application.Metadatas;

public sealed record MetadataListQuery<TEntity>;

public sealed record MetadataGetQuery<TEntity>(string Id);

public sealed record MetadataCreateCommand<TEntity, TRequest>(TRequest Request);

public sealed record MetadataUpdateCommand<TEntity, TRequest>(string Id, TRequest Request);

public sealed record MetadataDeleteCommand<TEntity>(string Id);

public sealed record MetadataAuditQuery<TEntity>(string Id);

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
