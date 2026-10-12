namespace AeroFids.Infrastructure.Identity;

public static class FidsRoles
{
    public const string Admin = "Admin";
}

public sealed class FidsUser
{
    public int Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = FidsRoles.Admin;
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }

    private FidsUser() { }

    public static FidsUser Create(string username, string passwordHash, string role, DateTimeOffset now) => new()
    {
        Username = username,
        PasswordHash = passwordHash,
        Role = role,
        IsActive = true,
        CreatedAt = now,
    };

    public void Update(string role, bool isActive)
    {
        Role = role;
        IsActive = isActive;
    }

    public void SetPassword(string passwordHash) => PasswordHash = passwordHash;
}
