using AeroFlight.Application.Tenancy;

namespace AeroFlight.Api.Infrastructure.Middleware;

// Resolves {tenantId} from the route and pins the request's tenant/DB connection; no-op if absent.
internal sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantStore store, ITenantContext tenantContext)
    {
        if (context.Request.RouteValues.TryGetValue("tenantId", out var raw)
            && raw is string tenantId && !string.IsNullOrWhiteSpace(tenantId))
        {
            var tenant = await store.FindAsync(tenantId, context.RequestAborted);
            if (tenant is null || !tenant.IsActive)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { error = $"Tenant '{tenantId}' not found." });
                return;
            }

            tenantContext.Set(tenant);
        }

        await next(context);
    }
}
