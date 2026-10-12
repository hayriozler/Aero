using Domain.Aggregates.Users;
using Domain.Common;

namespace Domain.Aggregates.Roles;

public sealed class Role : CompositeEntity
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    // EF Core
    private Role() { }

    public static Role Create(string name, string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        return new Role
        {
            Name        = name.ToLowerInvariant().Trim(),
            DisplayName = displayName.Trim()
        };
    }

    public void Update(string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        DisplayName = displayName.Trim();
    }

    public void AddPermission(int permissionId)
    {
        if (_rolePermissions.Any(rp => rp.PermissionId == permissionId)) return;
        _rolePermissions.Add(RolePermission.Create(Name, permissionId));
    }

    public void RemovePermission(int permissionId)
    {
        var entry = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
        if (entry is not null) _rolePermissions.Remove(entry);
    }

    public void SetPermissions(IEnumerable<int> permissionIds)
    {
        _rolePermissions.Clear();
        foreach (var id in permissionIds.Distinct())
            _rolePermissions.Add(RolePermission.Create(Name, id));
    }
}
