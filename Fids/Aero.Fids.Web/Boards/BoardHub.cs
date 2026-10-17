using Aero.Fids.Infrastructure.Boards;
using Microsoft.AspNetCore.SignalR;

namespace Aero.Fids.Web.Boards;

// A display client only ever identifies itself by tenant + name; its DisplayClient row (see
// DisplayClientRegistry) fully resolves what it shows — the physical screen carries no
// configuration of its own, and there is no anonymous/unregistered connection mode. A flight-list
// Type (DepartureFlightList/ArrivalFlightList/FullFlightList) is whole-airport, shared by every
// screen of that Type. A resource Type (Gate/Counter/DepartureBelt/ArrivalBelt/Spot) with a
// ResourceCode is per physical instance; the same Type with no ResourceCode is an overview of every
// code of that kind — either way, the server has already grouped/filtered everything by the time it
// reaches the client (see ResourceDatasetProjector), which only ever renders.
//
// This hub is the entire client-facing surface for board data — no REST fallback (see
// BoardEndpoints). A client that connects gets its initial snapshot pushed here, then deltas via
// "flightUpdated" (flight-list boards), "gateUpdated"/"beltUpdated"/"counterUpdated"/"spotUpdated"
// (single-instance resource boards), or repeated "boardSnapshot" full replaces (overview boards).
public sealed class BoardHub(
    IConnectedClientsRegistry connectedClients, IDisplayClientRegistry displayClients,
    IFlightBoardStore store, IFlightBoardBootstrapper bootstrapper, IFieldMappingRegistry fieldMappings) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var request = Context.GetHttpContext()?.Request;
        var tenantId = request?.Query["tenant"].ToString();
        var name = request?.Query["name"].ToString();

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(name))
        {
            await Clients.Caller.SendAsync("boardError", "Connect requires both 'tenant' and 'name' query parameters.", Context.ConnectionAborted);
            await base.OnConnectedAsync();
            return;
        }

        var display = await displayClients.FindByNameAsync(tenantId, name, Context.ConnectionAborted);
        if (display is not { IsActive: true })
        {
            var reason = display is null
                ? $"No display client named '{name}' is registered for tenant '{tenantId}'."
                : $"Display client '{name}' is registered but inactive.";
            await Clients.Caller.SendAsync("boardError", reason, Context.ConnectionAborted);
            await base.OnConnectedAsync();
            return;
        }

        var isResource = DisplayClientTypes.ResourceTypes.Contains(display.Type, StringComparer.OrdinalIgnoreCase);
        var groupName = isResource
            ? display.ResourceCode is { } code
                ? BoardGroups.NameForResource(tenantId, display.Type, code)
                : BoardGroups.NameForResourceOverview(tenantId, display.Type)
            : BoardGroups.NameForList(tenantId, display.Type);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        connectedClients.Register(Context.ConnectionId, new ConnectedClientInfo(
            tenantId, name, display.Type, display.Location, display.BankName, display.BankPosition, DateTimeOffset.UtcNow));

        var snapshot = await BoardEndpoints.BuildBoardAsync(
            tenantId, display.Type, display.ResourceCode, store, bootstrapper, fieldMappings, Context.ConnectionAborted);
        await Clients.Caller.SendAsync("boardSnapshot", snapshot, Context.ConnectionAborted);

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        connectedClients.Unregister(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
