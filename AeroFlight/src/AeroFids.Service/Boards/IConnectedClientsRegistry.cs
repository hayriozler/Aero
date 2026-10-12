using System.Collections.Concurrent;

namespace AeroFids.Service.Boards;

// Location/BankName/BankPosition are populated from the cached DisplayClient registry — purely
// informational for the admin connected-clients view.
public sealed record ConnectedClientInfo(
    string TenantId, string Name, string Type,
    string? Location, string? BankName, int? BankPosition, DateTimeOffset ConnectedAt);

public interface IConnectedClientsRegistry
{
    void Register(string connectionId, ConnectedClientInfo info);
    void Unregister(string connectionId);
    IReadOnlyList<ConnectedClientInfo> GetConnected(string tenantId);
}

// In-memory only — this is presence for the current process's live SignalR connections, not
// data anyone needs to survive a restart.
internal sealed class ConnectedClientsRegistry : IConnectedClientsRegistry
{
    private readonly ConcurrentDictionary<string, ConnectedClientInfo> _byConnectionId = new();

    public void Register(string connectionId, ConnectedClientInfo info) => _byConnectionId[connectionId] = info;

    public void Unregister(string connectionId) => _byConnectionId.TryRemove(connectionId, out _);

    public IReadOnlyList<ConnectedClientInfo> GetConnected(string tenantId) =>
        [.. _byConnectionId.Values.Where(c => string.Equals(c.TenantId, tenantId, StringComparison.OrdinalIgnoreCase))];
}
