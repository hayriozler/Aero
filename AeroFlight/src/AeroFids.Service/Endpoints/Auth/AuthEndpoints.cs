using System.Security.Claims;
using AeroFids.Infrastructure.Persistence;
using AeroFids.Service.Services;
using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Tenancy;
using AeroFlight.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroFids.Service.Endpoints.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (
            [FromForm] LoginFormModel form,
            ITenantStore tenantStore,
            IDbContextFactory<TenantDbContext> tenantDbFactory,
            IFidsDbContextFactory fidsDbFactory,
            IInterfaceSettingsService interfaceSettings,
            IPasswordHasher passwordHasher,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var tenant = await tenantStore.FindAsync(form.TenantId, ct);
            if (tenant is null || !tenant.IsActive)
                return Results.Redirect("/login?error=1");

            await using (var tenantDb = await tenantDbFactory.CreateDbContextAsync(ct))
            {
                var fidsEnabled = await tenantDb.TenantInterfaceSet.AnyAsync(
                    ti => ti.TenantId == tenant.Id && ti.InterfaceCode == InterfaceCodes.Fids && ti.IsEnabled, ct);
                if (!fidsEnabled)
                    return Results.Redirect("/login?error=1");
            }

            var fidsConnectionString = await interfaceSettings.FindAsync(tenant.Id, InterfaceSettingKeys.FidsConnectionString, ct)
                ?? throw new InvalidOperationException($"No '{InterfaceSettingKeys.FidsConnectionString}' configured for tenant '{tenant.Id}'.");

            await using var db = fidsDbFactory.Create(fidsConnectionString);
            var user = await db.FidsUserSet.FirstOrDefaultAsync(u => u.Username == form.Username && u.IsActive, ct);
            if (user is null || !passwordHasher.Verify(user.PasswordHash, form.Password))
                return Results.Redirect("/login?error=1");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role),
                new(FidsClaimTypes.Tenant, tenant.Id),
            };

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
