using AeroFids.Service.Boards;
using AeroFlight.Application.Tenancy;

namespace AeroFids.Service.Endpoints.ConnectedClients;

public static class ConnectedClientsEndpoints
{
    public static IEndpointRouteBuilder MapConnectedClientsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/connected-clients", (IConnectedClientsRegistry registry, ITenantContext tenant) =>
            Results.Ok(registry.GetConnected(tenant.TenantId))).RequireAuthorization();

        return app;
    }
}
