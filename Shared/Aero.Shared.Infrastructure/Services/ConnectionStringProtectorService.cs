using System.Security.Cryptography;
using System.Text;

namespace Aero.Shared.Infrastructure.Services;

// AES-256-GCM; output is base64(nonce|tag|cipher).
public sealed class ConnectionStringProtectorService(byte[] key) : IStringProtector
{
    private const int _nonceSize = 12;
    private const int _tagSize = 16;

    public string Protect(string plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(_nonceSize);
        var plain = Encoding.UTF8.GetBytes(plaintext);
        var cipher = new byte[plain.Length];
        var tag = new byte[_tagSize];

        using var aes = new AesGcm(key, _tagSize);
        aes.Encrypt(nonce, plain, cipher, tag);

        var combined = new byte[_nonceSize + _tagSize + cipher.Length];
        nonce.CopyTo(combined, 0);
        tag.CopyTo(combined, _nonceSize);
        cipher.CopyTo(combined, _nonceSize + _tagSize);
        return Convert.ToBase64String(combined);
    }

    public string Unprotect(string ciphertext)
    {
        var combined = Convert.FromBase64String(ciphertext);
        var nonce = combined.AsSpan(0, _nonceSize);
        var tag = combined.AsSpan(_nonceSize, _tagSize);
        var cipher = combined.AsSpan(_nonceSize + _tagSize);

        var plain = new byte[cipher.Length];
        using var aes = new AesGcm(key, _tagSize);
        aes.Decrypt(nonce, cipher, tag, plain);
        return Encoding.UTF8.GetString(plain);
    }
}
