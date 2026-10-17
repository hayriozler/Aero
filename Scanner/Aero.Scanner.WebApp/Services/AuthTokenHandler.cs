using Aero.Shared.Contracts.Dtos;
using Aero.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Aero.Scanner.WebApp.Services;

public sealed class AuthTokenHandler(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory clientFactory) : DelegatingHandler
{
    private static readonly TimeSpan _refreshThreshold = TimeSpan.FromMinutes(2);
    private static readonly JwtSecurityTokenHandler _jwtHandler = new();

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var ctx = httpContextAccessor.HttpContext;

        // In-request cache: if a refresh already happened this request, reuse the result
        var jwt = ctx?.Items["__refreshed_jwt"] as string
               ?? ctx?.User.FindFirst("jwt")?.Value;

        if (!string.IsNullOrEmpty(jwt) && IsNearExpiry(jwt))
        {
            var refreshToken = ctx!.Items["__refreshed_refresh_token"] as string
                            ?? ctx.User.FindFirst("refresh_token")?.Value;

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var refreshed = await TryRefreshAsync(ctx, refreshToken, cancellationToken);
                if (refreshed is not null)
                    jwt = refreshed;
            }
        }

        if (!string.IsNullOrEmpty(jwt))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        return await base.SendAsync(request, cancellationToken);
    }

    private static bool IsNearExpiry(string jwt)
    {
        try
        {
            var token = _jwtHandler.ReadJwtToken(jwt);
            return token.ValidTo <= DateTime.UtcNow.Add(_refreshThreshold);
        }
        catch
        {
            return true;
        }
    }

    private async Task<string?> TryRefreshAsync(
        HttpContext ctx, string refreshToken, CancellationToken ct)
    {
        try
        {
            var client = clientFactory.CreateClient("refresh");
            var response = await client.PostAsJsonAsync(
                "/api/auth/refresh", new RefreshRequest(refreshToken), ct);

            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(ct);
            if (result is null) return null;

            ctx.Items["__refreshed_jwt"]           = result.Token;
            ctx.Items["__refreshed_refresh_token"] = result.RefreshToken;

            await TryUpdateCookieAsync(ctx, result);

            return result.Token;
        }
        catch
        {
            return null;
        }
    }

    private static async Task TryUpdateCookieAsync(HttpContext ctx, LoginResponseDto result)
    {
        try
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, result.Username),
                new("display_name",  result.DisplayName),
                new("company_id",    result.CompanyCode),
                new("company_name",  result.CompanyName),
                new("company_type",  result.CompanyType),
                new("jwt",           result.Token),
                new("refresh_token", result.RefreshToken),
            };
            foreach (var role in result.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await ctx.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc   = DateTimeOffset.UtcNow.AddDays(7),
                });
        }
        catch
        {
            // Cookie update is best-effort; failure is acceptable in Blazor Server circuits
        }
    }
}
