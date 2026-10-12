using System.Collections.Concurrent;
using AeroFids.Infrastructure.Persistence;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace AeroFids.Service.Boards;

public interface IStatusMappingRegistry
{
    Task<StatusMapper> GetMapperAsync(string tenantId, CancellationToken ct);
    void Invalidate(string tenantId);
}

internal sealed class StatusMappingRegistry(IFidsDbContextFactory dbFactory, IInterfaceSettingsService interfaceSettings) : IStatusMappingRegistry
{
    private readonly ConcurrentDictionary<string, Task<StatusMapper>> _byTenant = new(StringComparer.OrdinalIgnoreCase);

    public async Task<StatusMapper> GetMapperAsync(string tenantId, CancellationToken ct)
    {
        var task = _byTenant.GetOrAdd(tenantId, id => LoadAsync(id, ct));
        try
        {
            return await task;
        }
        catch
        {
            _byTenant.TryRemove(new KeyValuePair<string, Task<StatusMapper>>(tenantId, task));
            throw;
        }
    }

    public void Invalidate(string tenantId) => _byTenant.TryRemove(tenantId, out _);

    private async Task<StatusMapper> LoadAsync(string tenantId, CancellationToken ct)
    {
        var connectionString = await interfaceSettings.FindAsync(tenantId, InterfaceSettingKeys.FidsConnectionString, ct)
            ?? throw new InvalidOperationException($"No '{InterfaceSettingKeys.FidsConnectionString}' configured for tenant '{tenantId}'.");
        await using var db = dbFactory.Create(connectionString);
        var mappings = await db.StatusMappingSet.AsNoTracking().ToListAsync(ct);
        return StatusMapper.Build(mappings);
    }
}
