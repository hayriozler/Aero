using Domain.Aggregates.Permissions;
using Domain.Common;

namespace Domain.Aggregates.Roles;

public sealed class RolePermission : CompositeEntity
{
    public string RoleName { get; private set; } = null!;
    public int PermissionId { get; private set; }

    public Role       Role       { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;

    private RolePermission() { }

    internal static RolePermission Create(string name, int permissionId) =>
        new() { RoleName = name, PermissionId = permissionId };
}
