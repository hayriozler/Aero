using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Identity;

public sealed class UserRole : AuditBase<long>
{
    public int UserId { get; private set; }
    public int RoleId { get; private set; }

    public User? User { get; private set; }
    public Role? Role { get; private set; }

    private UserRole() { }

    private UserRole(int userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public static UserRole Create(int userId, int roleId) => new(userId, roleId);
}
