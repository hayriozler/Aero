using Aero.Fids.Infrastructure.Boards;
using Aero.Fids.Infrastructure.Persistence;
using Aero.Shared.Application.Tenancy;
using Aero.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Aero.Fids.Web.Boards;

public interface IDisplayClientRegistry
{
    Task<DisplayClient?> FindByNameAsync(string tenantId, string name, CancellationToken ct);
    void Invalidate(string tenantId);
}

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
