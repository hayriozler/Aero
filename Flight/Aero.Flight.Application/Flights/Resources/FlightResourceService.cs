using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Resources;

// Single place every flight-resource source funnels through; Replace (full) and Patch (sparse) differ only in how they apply fields.
public interface IFlightResourceService<TResource> where TResource : ResourceBase
{
    Task<TResource> CreateAsync(long flightId, FlightResourceRequest request, CancellationToken ct);
    Task<TResource?> ReplaceAsync(long flightId, long resourceId, FlightResourceRequest request, CancellationToken ct);
    Task PatchAsync(TResource resource, FlightResourceOperation operation, CancellationToken ct);
    Task DeleteAsync(TResource resource, CancellationToken ct);
}

public sealed class FlightResourceService<TResource>(
    IAeroFlightDbContext db, IFlightResourceFactory<TResource> factory, IResourceDefaultStatusResolver statusResolver)
    : IFlightResourceService<TResource>
    where TResource : ResourceBase
{
    public async Task<TResource> CreateAsync(long flightId, FlightResourceRequest request, CancellationToken ct)
    {
        var resource = factory.Create(flightId, request.Code);
        ApplyFull(resource, request);
        await statusResolver.EnsureDefaultStatusAsync(resource, ct);
        db.Add(resource);
        await db.SaveChangesAsync(ct);
        return resource;
    }

    public async Task<TResource?> ReplaceAsync(long flightId, long resourceId, FlightResourceRequest request, CancellationToken ct)
    {
        var resource = await db.Query<TResource>().FirstOrDefaultAsync(r => r.Id == resourceId && r.FlightId == flightId, ct);
        if (resource is null)
            return null;

        ApplyFull(resource, request);
        await db.SaveChangesAsync(ct);
        return resource;
    }

    public async Task PatchAsync(TResource resource, FlightResourceOperation operation, CancellationToken ct)
    {
        if (operation.Code is not null) resource.UpdateCode(operation.Code);
        if (operation.BeginPlan is not null || operation.EndPlan is not null) resource.SetPlan(operation.BeginPlan, operation.EndPlan);
        if (operation.BeginActual is not null || operation.EndActual is not null) resource.RecordActual(operation.BeginActual, operation.EndActual);
        if (operation.StatusCode is not null) resource.UpdateStatus(operation.StatusCode);

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TResource resource, CancellationToken ct)
    {
        db.Remove(resource);
        await db.SaveChangesAsync(ct);
    }

    private static void ApplyFull(TResource resource, FlightResourceRequest r)
    {
        if (!string.Equals(resource.Code, r.Code, StringComparison.Ordinal))
            resource.UpdateCode(r.Code);
        resource.SetPlan(r.BeginPlan, r.EndPlan);
        resource.UpdateStatus(r.StatusCode);
        resource.RecordActual(r.BeginActual, r.EndActual);
        resource.UpdatePayer(r.PayerCompanyCode);
        resource.UpdateComplimentary(r.IsComplimentary);
        if (resource is OpsCounter counter)
            counter.UpdateCheckin(r.CheckinClassCode);
    }
}
