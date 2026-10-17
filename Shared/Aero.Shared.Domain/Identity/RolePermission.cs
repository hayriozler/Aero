namespace Aero.Shared.Domain.Identity;

public sealed class RolePermission
{
    public string RoleName { get; private set; } = string.Empty;
    public string PermissionCode { get; private set; } = string.Empty;

    private RolePermission() { }

    private RolePermission(string roleName, string permissionCode)
    {
        RoleName = roleName;
        PermissionCode = permissionCode;
    }

    public static RolePermission Create(string roleName, string permissionCode) => new(roleName, permissionCode);
}
