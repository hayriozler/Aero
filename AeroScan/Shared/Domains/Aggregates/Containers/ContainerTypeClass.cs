using Domain.Common;

namespace Domain.Aggregates.Containers;

public sealed class ContainerTypeClass : AuditableEntity
{
    public string TypeCode { get; private set; } = string.Empty;
    public string ClassCode { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private ContainerTypeClass() { }

    public static ContainerTypeClass Create(string typeCode, string classCode, string? description = null) => new()
    {
        TypeCode = typeCode.ToUpperInvariant().Trim(),
        ClassCode         = classCode.ToUpperInvariant().Trim(),
        Description       = description?.Trim(),
    };
    public void Update(string description) => Description = description.Trim();
}
