using AeroFlight.Application.Interfaces;
using System.Security.Cryptography;

namespace AeroFlight.Infrastructure.Identity;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int _saltSize = 16;
    private const int _hashSize = 32;
    private const int _iterations = 310_000;
    private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _algorithm, _hashSize);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string hashedPassword, string password)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _algorithm, _hashSize);
        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }
}
