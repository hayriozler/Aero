using Aero.Flights.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aero.Flights.Web.Endpoints.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (
            [FromForm] LoginFormModel form, IApiClient api, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await api.LoginAsync(form.TenantId, form.Username, form.Password, ct);
            if (result is null)
                return Results.Redirect("/login?error=1");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, result.Username),
                new(TenantClaimTypes.Tenant, form.TenantId),
                new("TimeZoneManager", result.TimeZone),
                new("tenantDateFormat", result.DateFormat),
                new("tenantTimeFormat", result.TimeFormat),
                new("api_token", result.Token),
                new("api_token_expires", result.ExpiresAt.ToString("O")),
            };
            claims.AddRange(result.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            var isLocalUrl = form.ReturnUrl is ['/', not '/', ..];
            return Results.Redirect(isLocalUrl ? form.ReturnUrl! : "/");
        });

        app.MapPost("/logout", async (HttpContext ctx) =>
        {
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        }).DisableAntiforgery();

        return app;
    }
}

public sealed record LoginFormModel(string TenantId, string Username, string Password, string? ReturnUrl = null);
