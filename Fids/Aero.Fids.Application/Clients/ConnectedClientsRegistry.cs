using System.Collections.Concurrent;

namespace Aero.Fids.Application.Clients;

internal sealed class ConnectedClientsRegistry : IConnectedClientsRegistry
{
    private readonly ConcurrentDictionary<string, ConnectedClientInfo> _byConnectionId = new();

    public void Register(string connectionId, ConnectedClientInfo info) => _byConnectionId[connectionId] = info;

    public void Unregister(string connectionId) => _byConnectionId.TryRemove(connectionId, out _);

    public IReadOnlyList<ConnectedClientInfo> GetConnected(string tenantId) =>
        [.. _byConnectionId.Values.Where(c => string.Equals(c.TenantId, tenantId, StringComparison.OrdinalIgnoreCase))];
}
