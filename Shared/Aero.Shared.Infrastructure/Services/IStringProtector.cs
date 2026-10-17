namespace Aero.Shared.Infrastructure.Services;

public interface IStringProtector
{
    string Protect(string plaintext);
    string Unprotect(string ciphertext);
}
