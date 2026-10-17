using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops.Routing;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Routings;

public sealed record ListRoutingsQuery<TRouting>(long FlightId) where TRouting : OpsRouting;
public sealed record UpdateRoutingCommand<TRouting>(long FlightId, long RoutingId, OpsRoutingRequest Request) where TRouting : OpsRouting;
public sealed record DeleteRoutingCommand<TRouting>(long FlightId, long RoutingId) where TRouting : OpsRouting;

public enum DeleteRoutingOutcome { Deleted, NotFound, LastRemainingLeg }
public sealed record AddRoutingCommand<TRouting>(long FlightId, OpsRoutingRequest Request) where TRouting : OpsRouting;

public enum AddRoutingOutcome { Created }
public sealed record AddRoutingResult(AddRoutingOutcome Outcome, OpsRoutingResponse? Routing);

// Leg 0 is the flight's own Origin/Destination — it lives here too (so it can share in country-type
// resolution and routing summaries) and is listed alongside the other legs for a complete itinerary
// view, but it's edited only via the main flight form, never through this generic leg CRUD, so
// Add/Update/Delete below still exclude it.
public sealed class ListRoutingsQueryHandler<TRouting>(IAeroFlightDbContext db)
    : IQueryHandler<ListRoutingsQuery<TRouting>, IReadOnlyList<OpsRoutingResponse>>
    where TRouting : OpsRouting
{
    public async Task<IReadOnlyList<OpsRoutingResponse>> HandleAsync(ListRoutingsQuery<TRouting> query, CancellationToken ct)
    {
        var items = await db.Query<TRouting>()
            .Where(r => r.FlightId == query.FlightId)
            .OrderBy(r => r.LegNumber)
            .ToListAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    internal static OpsRoutingResponse ToResponse(TRouting r) => new(r.Id, r.FlightId, r.Code, r.LegNumber);
}

// No hard cap — more than 3 additional legs is unusual so the UI warns, but a genuine multi-sector
// rotation shouldn't be blocked from being recorded.
public sealed class AddRoutingCommandHandler<TRouting>(IAeroFlightDbContext db, IRoutingFactory<TRouting> factory, IRoutingCountryTypeUpdater<TRouting> countryTypeUpdater)
    : ICommandHandler<AddRoutingCommand<TRouting>, AddRoutingResult>
    where TRouting : OpsRouting
{
    public async Task<AddRoutingResult> HandleAsync(AddRoutingCommand<TRouting> command, CancellationToken ct)
    {
        var maxLeg = await db.Query<TRouting>()
            .Where(r => r.FlightId == command.FlightId)
            .Select(r => (int?)r.LegNumber)
            .MaxAsync(ct);
        var nextLeg = (maxLeg ?? 0) + 1;

        var routing = factory.Create(command.FlightId, command.Request.AirportCode, nextLeg);
        db.Add(routing);
        await db.SaveChangesAsync(ct);
        await countryTypeUpdater.RefreshAsync(command.FlightId, ct);
        return new AddRoutingResult(AddRoutingOutcome.Created, ListRoutingsQueryHandler<TRouting>.ToResponse(routing));
    }
}

public sealed class UpdateRoutingCommandHandler<TRouting>(IAeroFlightDbContext db, IRoutingCountryTypeUpdater<TRouting> countryTypeUpdater)
    : ICommandHandler<UpdateRoutingCommand<TRouting>, OpsRoutingResponse?>
    where TRouting : OpsRouting
{
    public async Task<OpsRoutingResponse?> HandleAsync(UpdateRoutingCommand<TRouting> command, CancellationToken ct)
    {
        var routing = await db.Query<TRouting>()
            .FirstOrDefaultAsync(r => r.Id == command.RoutingId && r.FlightId == command.FlightId && r.LegNumber > 0, ct);
        if (routing is null)
            return null;

        routing.UpdateLeg(command.Request.AirportCode, routing.LegNumber);
        await db.SaveChangesAsync(ct);
        await countryTypeUpdater.RefreshAsync(routing.FlightId, ct);
        return ListRoutingsQueryHandler<TRouting>.ToResponse(routing);
    }
}

// A flight must always retain at least one additional leg beyond leg 0 (Origin/Destination) once
// it has one — deleting the last remaining leg>0 is rejected rather than silently leaving the
// flight with only its Origin/Destination.
public sealed class DeleteRoutingCommandHandler<TRouting>(IAeroFlightDbContext db, IRoutingCountryTypeUpdater<TRouting> countryTypeUpdater)
    : ICommandHandler<DeleteRoutingCommand<TRouting>, DeleteRoutingOutcome>
    where TRouting : OpsRouting
{
    public async Task<DeleteRoutingOutcome> HandleAsync(DeleteRoutingCommand<TRouting> command, CancellationToken ct)
    {
        var routing = await db.Query<TRouting>()
            .FirstOrDefaultAsync(r => r.Id == command.RoutingId && r.FlightId == command.FlightId && r.LegNumber > 0, ct);
        if (routing is null)
            return DeleteRoutingOutcome.NotFound;

        var hasOtherLeg = await db.Query<TRouting>()
            .AnyAsync(r => r.FlightId == routing.FlightId && r.LegNumber > 0 && r.Id != routing.Id, ct);
        if (!hasOtherLeg)
            return DeleteRoutingOutcome.LastRemainingLeg;

        var flightId = routing.FlightId;
        db.Remove(routing);
        await db.SaveChangesAsync(ct);
        await countryTypeUpdater.RefreshAsync(flightId, ct);
        return DeleteRoutingOutcome.Deleted;
    }
}
