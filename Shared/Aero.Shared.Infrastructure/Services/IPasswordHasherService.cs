namespace Aero.Shared.Infrastructure.Services;

public interface IPasswordHasherService
{
    string Hash(string password);
    bool Verify(string hashedPassword, string password);
}
