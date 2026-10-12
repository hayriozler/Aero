using System.Collections.Concurrent;
using AeroFids.Infrastructure.Boards;
using AeroFids.Infrastructure.Persistence;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace AeroFids.Service.Boards;

public interface IDisplayClientRegistry
{
    Task<DisplayClient?> FindByNameAsync(string tenantId, string name, CancellationToken ct);
    void Invalidate(string tenantId);
}

// BoardHub has no per-request tenant context (a SignalR circuit, same story as IFieldMappingRegistry),
// so this looks a display client up by an explicit tenant id and caches the tenant's active
// clients until a write invalidates them.
internal sealed class DisplayClientRegistry(IFidsDbContextFactory dbFactory, IInterfaceSettingsService interfaceSettings) : IDisplayClientRegistry
{
    private readonly ConcurrentDictionary<string, Task<IReadOnlyList<DisplayClient>>> _byTenant = new(StringComparer.OrdinalIgnoreCase);

    public async Task<DisplayClient?> FindByNameAsync(string tenantId, string name, CancellationToken ct)
    {
        var task = _byTenant.GetOrAdd(tenantId, id => LoadAsync(id, ct));
        IReadOnlyList<DisplayClient> clients;
        try
        {
            clients = await task;
        }
        catch
        {
            _byTenant.TryRemove(new KeyValuePair<string, Task<IReadOnlyList<DisplayClient>>>(tenantId, task));
            throw;
        }

        return clients.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public void Invalidate(string tenantId) => _byTenant.TryRemove(tenantId, out _);

    private async Task<IReadOnlyList<DisplayClient>> LoadAsync(string tenantId, CancellationToken ct)
    {
        var connectionString = await interfaceSettings.FindAsync(tenantId, InterfaceSettingKeys.FidsConnectionString, ct)
            ?? throw new InvalidOperationException($"No '{InterfaceSettingKeys.FidsConnectionString}' configured for tenant '{tenantId}'.");
        await using var db = dbFactory.Create(connectionString);
        return await db.DisplayClientSet.AsNoTracking().Where(c => c.IsActive).ToListAsync(ct);
    }
}
