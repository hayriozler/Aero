namespace Aero.Fids.Application.FieldMappings;

public interface IFieldMappingRegistry
{
    Task<FlightFieldMapper> GetMapperAsync(string tenantId, CancellationToken ct);
    void Invalidate(string tenantId);
}
