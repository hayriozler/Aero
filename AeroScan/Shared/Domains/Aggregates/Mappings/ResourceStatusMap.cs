namespace Domain.Aggregates.Mappings;

public sealed class ResourceStatusMap
{
    public string SourceResourceName { get; private set; } = string.Empty;
    public string SourceResourceStatus { get; private set; } = string.Empty;
    public string TargetResourceStatus { get; private set; } = string.Empty;

    private ResourceStatusMap() { }

    public static ResourceStatusMap Create(string sourceResourceName, string sourceResourceStatus, string targetResourceStatus) => new()
    {
        SourceResourceName   = sourceResourceName.Trim(),
        SourceResourceStatus = sourceResourceStatus.Trim(),
        TargetResourceStatus = targetResourceStatus.Trim(),
    };

    public void Update(string targetResourceStatus) => TargetResourceStatus = targetResourceStatus.Trim();
}
