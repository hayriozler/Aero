using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Resources;

public sealed record ListFlightResourcesQuery<TResource>(long FlightId) where TResource : ResourceBase;
public sealed record AddFlightResourceCommand<TResource>(long FlightId, FlightResourceRequest Request) where TResource : ResourceBase;
public sealed record UpdateFlightResourceCommand<TResource>(long FlightId, long ResourceId, FlightResourceRequest Request) where TResource : ResourceBase;
public sealed record DeleteFlightResourceCommand<TResource>(long FlightId, long ResourceId) where TResource : ResourceBase;
public sealed record BulkUpdateResourceStatusCommand<TResource>(long FlightId, string StatusCode) where TResource : ResourceBase;

public sealed class ListFlightResourcesQueryHandler<TResource>(IAeroFlightDbContext db)
    : IQueryHandler<ListFlightResourcesQuery<TResource>, IReadOnlyList<FlightResourceResponse>>
    where TResource : ResourceBase
{
    public async Task<IReadOnlyList<FlightResourceResponse>> HandleAsync(ListFlightResourcesQuery<TResource> query, CancellationToken ct)
    {
        var items = await db.Query<TResource>()
            .Where(r => r.FlightId == query.FlightId)
            .OrderBy(r => r.BeginPlan)
            .ToListAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    internal static FlightResourceResponse ToResponse(TResource e) => new(
       e.Id, e.FlightId, e.Code, e.LastCode, e.BeginPlan, e.EndPlan, e.BeginActual, e.EndActual,
        e.StatusCode, e.PayerCompanyCode, e.IsComplimentary, (e as OpsCounter)?.CheckinClassCode);
}

// Gates/Belts/Counters (+ status/begin/end) on ArrivalFlight/DepartureFlight are computed straight
// from these resource rows (see ResourceRollup), so no separate sync step is needed here — saving
// the resource change is enough for the flight's summary to already reflect it.
public sealed class AddFlightResourceCommandHandler<TResource>(IFlightResourceService<TResource> resourceService)
    : ICommandHandler<AddFlightResourceCommand<TResource>, FlightResourceResponse>
    where TResource : ResourceBase
{
    public async Task<FlightResourceResponse> HandleAsync(AddFlightResourceCommand<TResource> command, CancellationToken ct)
    {
        var resource = await resourceService.CreateAsync(command.FlightId, command.Request, ct);
        return ListFlightResourcesQueryHandler<TResource>.ToResponse(resource);
    }
}

public sealed class UpdateFlightResourceCommandHandler<TResource>(IFlightResourceService<TResource> resourceService)
    : ICommandHandler<UpdateFlightResourceCommand<TResource>, FlightResourceResponse?>
    where TResource : ResourceBase
{
    public async Task<FlightResourceResponse?> HandleAsync(UpdateFlightResourceCommand<TResource> command, CancellationToken ct)
    {
        var resource = await resourceService.ReplaceAsync(command.FlightId, command.ResourceId, command.Request, ct);
        return resource is null ? null : ListFlightResourcesQueryHandler<TResource>.ToResponse(resource);
    }
}

public sealed class DeleteFlightResourceCommandHandler<TResource>(IAeroFlightDbContext db, IFlightResourceService<TResource> resourceService)
    : ICommandHandler<DeleteFlightResourceCommand<TResource>, bool>
    where TResource : ResourceBase
{
    public async Task<bool> HandleAsync(DeleteFlightResourceCommand<TResource> command, CancellationToken ct)
    {
        var resource = await db.Query<TResource>().FirstOrDefaultAsync(r => r.Id == command.ResourceId && r.FlightId == command.FlightId, ct);
        if (resource is null)
            return false;

        await resourceService.DeleteAsync(resource, ct);
        return true;
    }
}

public sealed class BulkUpdateResourceStatusCommandHandler<TResource>(IAeroFlightDbContext db, IResourceDefaultStatusResolver statusResolver)
    : ICommandHandler<BulkUpdateResourceStatusCommand<TResource>, IReadOnlyList<FlightResourceResponse>>
    where TResource : ResourceBase
{
    public async Task<IReadOnlyList<FlightResourceResponse>> HandleAsync(BulkUpdateResourceStatusCommand<TResource> command, CancellationToken ct)
    {
        var resources = await db.Query<TResource>().Where(r => r.FlightId == command.FlightId).ToListAsync(ct);
        var status = await statusResolver.FindByCodeAsync<TResource>(command.StatusCode, ct);
        var now = DateTimeOffset.UtcNow;
        foreach (var resource in resources)
        {
            resource.UpdateStatus(command.StatusCode);
            if (status?.IsOpenValue == true)
                resource.RecordActual(now, resource.EndActual);
            else if (status?.IsCloseValue == true)
                resource.RecordActual(resource.BeginActual, now);
        }

        await db.SaveChangesAsync(ct);
        return [.. resources.Select(ListFlightResourcesQueryHandler<TResource>.ToResponse)];
    }
}
