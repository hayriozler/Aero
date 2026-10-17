using Aero.Shared.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Aero.Shared.Infrastructure.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private const int _expiryMinutes = 15;
    public (string Token, DateTime ExpiresAt) GenerateToken(JwtOptions jwtOptions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,        jwtOptions.User.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, jwtOptions.User.Username),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString())
        };

        foreach (var role in jwtOptions.User.Roles)
        {
            claims.Add(new Claim("role", role.RoleName));
        }
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
