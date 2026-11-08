namespace Aero.Shared.Domain.Identity.Audits;

public static class Permissions
{
    public const string UserManage = "user.manage";
    public const string RoleManage = "role.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
   [
        (UserManage, "Manage users"),
        (RoleManage, "Manage roles and permissions")
    ];
}
