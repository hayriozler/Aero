using Domain.Common;

namespace Domain.Aggregates.Containers;

public sealed class ContainerType : AuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsAllDestination { get; private set; }
    public bool IsTransfer { get; private set; }
    private ContainerType() { }

    public static ContainerType Create(string code, string description, bool isAllDestination, bool isTransfer) => new()
    {
        Code             = code.ToUpperInvariant().Trim(),
        Description      = description.Trim(),
        IsAllDestination = isAllDestination,
        IsTransfer       = isTransfer,
    };

    public void Update(string description, bool isAllDestination, bool isTransfer)
    {
        Description      = description.Trim();
        IsAllDestination = isAllDestination;
        IsTransfer       = isTransfer;
    }
}
