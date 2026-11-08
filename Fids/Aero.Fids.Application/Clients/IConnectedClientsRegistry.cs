namespace Aero.Fids.Application.Clients;

public interface IConnectedClientsRegistry
{
    void Register(string connectionId, ConnectedClientInfo info);
    void Unregister(string connectionId);
    IReadOnlyList<ConnectedClientInfo> GetConnected(string tenantId);
}
