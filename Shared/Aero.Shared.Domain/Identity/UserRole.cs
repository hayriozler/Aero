namespace Aero.Shared.Domain.Identity;

public sealed class UserRole : CompositeEntity
{
    public int UserId { get; private set; }
    public string RoleName { get; private set; } = string.Empty;

    public User? User { get; private set; }
    public Role? Role { get; private set; }

    private UserRole() { }

    private UserRole(int userId, string roleName)
    {
        UserId = userId;
        RoleName = roleName;
    }

    public static UserRole Create(int userId, string roleName) => new(userId, roleName);
}
