using Aero.Shared.Domain;

namespace Aero.Shared.Infrastructure.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(JwtOptions jwtOption);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
}
