using Domain.Aggregates.Roles;
using Domain.Common;

namespace Domain.Aggregates.Users;

/// <summary>User ↔ Role many-to-many junction</summary>
public sealed class UserRole : CompositeEntity
{
    public int UserId { get; private set; }
    public string RoleName { get; private set; } = null!;

    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    // EF Core
    private UserRole() { }

    internal static UserRole Create(int userId, string roleName) =>
        new() { UserId = userId, RoleName = roleName };
}
