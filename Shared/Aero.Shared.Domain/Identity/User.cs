using Aero.Flights.Domain.Common;

namespace Aero.Shared.Domain.Identity;

public class User : AuditBase<int>
{
    private readonly List<UserRole> _userRoles = [];
    private readonly List<RefreshToken> _refreshTokens = [];
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? RealName { get; private set; }
    public DateTimeOffset? PasswordExpirationDate { get; private set; }
    public int? MultipleLoginsAllowed { get; private set; }
    public DateTimeOffset? ExpirationDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public void UpdateProfile(string? realName) => RealName = realName;

    public void ChangePasswordHash(string passwordHash, DateTimeOffset? expirationDate = null)
    {
        PasswordHash = passwordHash;
        PasswordExpirationDate = expirationDate;
    }
    public void AssignRole(string roleName)
    {
        if (_userRoles.Any(ur => ur.RoleName == roleName)) return;
        _userRoles.Add(UserRole.Create(Id, roleName));
    }
    public void RemoveRole(string roleName)
    {
        var entry = _userRoles.FirstOrDefault(ur => ur.RoleName == roleName);
        if (entry is not null) _userRoles.Remove(entry);
    }
    public Result UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            return Result.Failure("Password hash cannot be empty.");

        PasswordHash = newPasswordHash;
        return Result.Success();
    }

    public static User Create(string username, string passwordHash, string? realName = null, DateTimeOffset? passwordExpirationDate = null, int? multipleLoginsAllowed = null, DateTimeOffset? expirationDate = null)
    {
        return new User
        {
            Username = username,
            PasswordHash = passwordHash,
            RealName = realName,
            PasswordExpirationDate = passwordExpirationDate,
            MultipleLoginsAllowed = multipleLoginsAllowed,
            ExpirationDate = expirationDate
        };
    }
    public void SetExpiration(DateTimeOffset? expirationDate) => ExpirationDate = expirationDate;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;
}

