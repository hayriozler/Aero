namespace Aero.Shared.Domain.Identity;

public sealed class RefreshToken : Entity<long>
{
    public int UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public User User { get; private set; } = null!;

    private RefreshToken() { }

    public static RefreshToken Create(int userId, string tokenHash, DateTime expiresAt) => new()
    {
        UserId    = userId,
        TokenHash = tokenHash,
        ExpiresAt = expiresAt,
        CreatedAt = DateTime.UtcNow
    };

    public void Revoke() => RevokedAt = DateTime.UtcNow;
}

