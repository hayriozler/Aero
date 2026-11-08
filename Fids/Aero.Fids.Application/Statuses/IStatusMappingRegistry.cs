namespace Aero.Fids.Application.Statuses;

public interface IStatusMappingRegistry
{
    Task<StatusMapper> GetMapperAsync(string tenantId, CancellationToken ct);
    void Invalidate(string tenantId);
}
