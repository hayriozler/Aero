using Aero.Fids.Domain.Boards;

namespace Aero.Fids.Application.DisplayClients;

public interface IDisplayClientRegistry
{
    Task<DisplayClient?> FindByNameAsync(string tenantId, string name, CancellationToken ct);
    void Invalidate(string tenantId);
}
