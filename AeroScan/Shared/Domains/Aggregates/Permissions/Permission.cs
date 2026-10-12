using Domain.Aggregates.Roles;
using Domain.Common;

namespace Domain.Aggregates.Permissions;

public sealed class Permission : AuditableEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Group { get; private set; } = string.Empty;

    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Permission() { }

    public static Permission Create(string name, string displayName, string group, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(group);

        return new Permission
        {
            Name        = name.ToLowerInvariant().Trim(),
            DisplayName = displayName.Trim(),
            Group       = group.Trim(),
            Description = description?.Trim()
        };
    }

    public void Update(string displayName, string group, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(group);
        DisplayName = displayName.Trim();
        Group       = group.Trim();
        Description = description?.Trim();
    }
}
