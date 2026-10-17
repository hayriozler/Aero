using Aero.Flights.Contracts.Identity;

namespace Aero.Flights.Api.Infrastructure;

public static class TenantAuthorizationExtensions
{
    public static RouteGroupBuilder RequireTenantAccessFilter(
        this RouteGroupBuilder group)
    {
        group.AddEndpointFilter(async (ctx, next) =>
        {
            var http = ctx.HttpContext;

            if (http.User.Identity?.IsAuthenticated == true)
            {
                var routeTenant =
                    http.Request.RouteValues["tenantId"] as string;

                var tokenTenant = http.User.FindFirst(TenantClaimTypes.Tenant)?.Value;

                if (!string.Equals(routeTenant, tokenTenant, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.Json(new { error = "Tenant mismatch." },statusCode: StatusCodes.Status403Forbidden);
                }
            }
            return await next(ctx);
        });

        return group;
    }
}
