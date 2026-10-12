using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AeroFlight.Infrastructure.Identity;

internal sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    public (string Token, DateTimeOffset ExpiresAt) GenerateToken(int userId, string username, string tenant, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, username),
        };
        return BuildToken(claims, tenant, roles);
    }

    public (string Token, DateTimeOffset ExpiresAt) GenerateServiceToken(string clientId, string tenant, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, clientId),
        };
        return BuildToken(claims, tenant, roles);
    }

    private (string Token, DateTimeOffset ExpiresAt) BuildToken(List<Claim> claims, string tenant, IEnumerable<string> roles)
    {
        var opts = options.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Key));
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(opts.ExpiryMinutes);

        claims.Add(new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new(TenantClaimTypes.Tenant, tenant));
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: opts.Issuer,
            audience: opts.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
