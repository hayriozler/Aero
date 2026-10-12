using AeroFlight.Application.Configuration;
using AeroFlight.Application.Interfaces;
using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.Resources;

public interface IResourceDefaultStatusResolver
{
    Task<string?> ResolveDefaultStatusCodeAsync<TResource>(CancellationToken ct) where TResource : ResourceBase;
    Task<string?> ResolveCloseStatusCodeAsync<TResource>(CancellationToken ct) where TResource : ResourceBase;
    Task<IResourceStatus?> FindByCodeAsync<TResource>(string code, CancellationToken ct) where TResource : ResourceBase;

    Task EnsureDefaultStatusAsync<TResource>(TResource resource, CancellationToken ct) where TResource : ResourceBase;
}

public sealed class ResourceDefaultStatusResolver(IAeroFlightDbContext db, IAeroConfigService aeroConfig) : IResourceDefaultStatusResolver
{
    public async Task<string?> ResolveDefaultStatusCodeAsync<TResource>(CancellationToken ct) where TResource : ResourceBase
    {
        var overrideCode = await aeroConfig.GetAsync($"Resources.{KindOf<TResource>()}.DefaultStatusCode", ct);
        if (!string.IsNullOrWhiteSpace(overrideCode))
            return overrideCode;

        return (await FindAsync<TResource>(code: null, isDefault: true, ct))?.Code;
    }

    public async Task<string?> ResolveCloseStatusCodeAsync<TResource>(CancellationToken ct) where TResource : ResourceBase =>
        (await FindCloseAsync<TResource>(ct))?.Code;

    public Task<IResourceStatus?> FindByCodeAsync<TResource>(string code, CancellationToken ct) where TResource : ResourceBase =>
        FindAsync<TResource>(code, isDefault: false, ct);

    public async Task EnsureDefaultStatusAsync<TResource>(TResource resource, CancellationToken ct) where TResource : ResourceBase
    {
        if (string.IsNullOrWhiteSpace(resource.StatusCode))
            resource.UpdateStatus(await ResolveDefaultStatusCodeAsync<TResource>(ct));
    }

    private static string KindOf<TResource>() where TResource : ResourceBase
    {
        if (typeof(TResource) == typeof(OpsArrivalGate)) return "ArrivalGate";
        if (typeof(TResource) == typeof(OpsDepartureGate)) return "DepartureGate";
        if (typeof(TResource) == typeof(OpsArrivalBelt)) return "ArrivalBelt";
        if (typeof(TResource) == typeof(OpsDepartureBelt)) return "DepartureBelt";
        if (typeof(TResource) == typeof(OpsCounter)) return "Counter";

        throw new InvalidOperationException($"No status table mapped for resource type '{typeof(TResource).Name}'.");
    }

    private async Task<IResourceStatus?> FindCloseAsync<TResource>(CancellationToken ct) where TResource : ResourceBase
    {
        if (typeof(TResource) == typeof(OpsArrivalGate) || typeof(TResource) == typeof(OpsDepartureGate))
            return await db.Query<GateStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.IsCloseValue, ct);
        if (typeof(TResource) == typeof(OpsArrivalBelt))
            return await db.Query<ArrivalBeltStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.IsCloseValue, ct);
        if (typeof(TResource) == typeof(OpsDepartureBelt))
            return await db.Query<DepartureBeltStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.IsCloseValue, ct);
        if (typeof(TResource) == typeof(OpsCounter))
            return await db.Query<CounterStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.IsCloseValue, ct);

        throw new InvalidOperationException($"No status table mapped for resource type '{typeof(TResource).Name}'.");
    }

    private async Task<IResourceStatus?> FindAsync<TResource>(string? code, bool isDefault, CancellationToken ct)
        where TResource : ResourceBase
    {
        if (typeof(TResource) == typeof(OpsArrivalGate) || typeof(TResource) == typeof(OpsDepartureGate))
            return isDefault
                ? await db.Query<GateStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault, ct)
                : await db.Query<GateStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.Code == code, ct);
        if (typeof(TResource) == typeof(OpsArrivalBelt))
            return isDefault
                ? await db.Query<ArrivalBeltStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault, ct)
                : await db.Query<ArrivalBeltStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.Code == code, ct);
        if (typeof(TResource) == typeof(OpsDepartureBelt))
            return isDefault
                ? await db.Query<DepartureBeltStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault, ct)
                : await db.Query<DepartureBeltStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.Code == code, ct);
        if (typeof(TResource) == typeof(OpsCounter))
            return isDefault
                ? await db.Query<CounterStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.IsDefault, ct)
                : await db.Query<CounterStatus>().AsNoTracking().FirstOrDefaultAsync(s => s.Code == code, ct);

        throw new InvalidOperationException($"No status table mapped for resource type '{typeof(TResource).Name}'.");
    }
}
