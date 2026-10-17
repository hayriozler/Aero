using Aero.Fids.Web.Boards;

namespace Aero.Fids.Web.Endpoints.ConnectedClients;

public static class ConnectedClientsEndpoints
{
    public static IEndpointRouteBuilder MapConnectedClientsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/connected-clients", (IConnectedClientsRegistry registry, ITenantContext tenant) =>
            Results.Ok(registry.GetConnected(tenant.TenantId))).RequireAuthorization();

        return app;
    }
}
