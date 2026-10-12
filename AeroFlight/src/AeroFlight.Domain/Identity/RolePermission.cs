namespace AeroFlight.Domain.Identity;

public sealed class RolePermission
{
    public int RoleId { get; private set; }
    public string PermissionCode { get; private set; } = string.Empty;

    private RolePermission() { }

    private RolePermission(int roleId, string permissionCode)
    {
        RoleId = roleId;
        PermissionCode = permissionCode;
    }

    public static RolePermission Create(int roleId, string permissionCode) => new(roleId, permissionCode);
}
