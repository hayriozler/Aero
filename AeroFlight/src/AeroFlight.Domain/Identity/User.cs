using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Identity;

public sealed class User : AuditBase<int>
{
    private readonly List<UserRole> _userRoles = [];

    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? RealName { get; private set; }
    public DateTimeOffset? PasswordExpirationDate { get; private set; }
    public int? MultipleLoginsAllowed { get; private set; }
    public DateTimeOffset? ExpirationDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    private User() { }

    private User(string username, string passwordHash, string? realName)
    {
        Username = username;
        PasswordHash = passwordHash;
        RealName = realName;
    }

    public static User Create(string username, string passwordHash, string? realName = null)
        => new(username, passwordHash, realName);

    public void UpdateProfile(string? realName) => RealName = realName;

    public void ChangePasswordHash(string passwordHash, DateTimeOffset? expirationDate = null)
    {
        PasswordHash = passwordHash;
        PasswordExpirationDate = expirationDate;
    }

    public void SetExpiration(DateTimeOffset? expirationDate) => ExpirationDate = expirationDate;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
