using Aero.Scanner.Domain.Aggregates.Companies;
using Aero.Shared.Domain;

namespace Aero.Scanner.Domain.Aggregates.Users;

public sealed class User : AuditableEntity<int>
{
    public string Username { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string CompanyCode { get; private set; } = string.Empty;
    public DateTime? LastLoginAt { get; private set; }
    public Company Company { get; private set; } = null!;

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public IEnumerable<string> GetRoleNames() =>
        _userRoles.Where(ur => ur.Role is not null).Select(ur => ur.Role.Name);

    private User() { }

    public static User Create(string username, string displayName, string passwordHash, string companyCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new User
        {
            Username = username.ToLowerInvariant().Trim(),
            DisplayName = displayName.Trim(),
            PasswordHash = passwordHash,
            CompanyCode = companyCode
        };
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

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;

    public void UpdateDetails(string displayName)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
            DisplayName = displayName.Trim();
    }
}
