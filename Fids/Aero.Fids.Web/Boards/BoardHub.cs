using Aero.Fids.Application.Clients;
using Aero.Fids.Application.DisplayClients;
using Aero.Fids.Application.FieldMappings;
using Aero.Fids.Domain.Boards;
using Aero.Fids.Web.Endpoints.Boards;
using Microsoft.AspNetCore.SignalR;

namespace Aero.Fids.Web.Boards;

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
