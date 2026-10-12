using System.Collections.Concurrent;
using AeroFids.Infrastructure.Persistence;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace AeroFids.Service.Boards;

public interface IFieldMappingRegistry
{
    Task<FlightFieldMapper> GetMapperAsync(string tenantId, CancellationToken ct);
    void Invalidate(string tenantId);
}

// FlightUpdatesConsumer is a singleton background service and can't afford a DB round trip per
// flight event, so the compiled mapper is cached per tenant until explicitly invalidated by a write.
internal sealed class FieldMappingRegistry(IFidsDbContextFactory dbFactory, IInterfaceSettingsService interfaceSettings) : IFieldMappingRegistry
{
    private readonly ConcurrentDictionary<string, Task<FlightFieldMapper>> _byTenant = new(StringComparer.OrdinalIgnoreCase);

    public async Task<FlightFieldMapper> GetMapperAsync(string tenantId, CancellationToken ct)
    {
        var task = _byTenant.GetOrAdd(tenantId, id => LoadAsync(id, ct));
        try
        {
            return await task;
        }
        catch
        {
            _byTenant.TryRemove(new KeyValuePair<string, Task<FlightFieldMapper>>(tenantId, task));
            throw;
        }
    }

    public void Invalidate(string tenantId) => _byTenant.TryRemove(tenantId, out _);

    private async Task<FlightFieldMapper> LoadAsync(string tenantId, CancellationToken ct)
    {
        var connectionString = await interfaceSettings.FindAsync(tenantId, InterfaceSettingKeys.FidsConnectionString, ct)
            ?? throw new InvalidOperationException($"No '{InterfaceSettingKeys.FidsConnectionString}' configured for tenant '{tenantId}'.");
        await using var db = dbFactory.Create(connectionString);
        var mappings = await db.FieldMappingSet.AsNoTracking().ToListAsync(ct);
        return FlightFieldMapper.Build(mappings);
    }
}
