using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Domain.Ops;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Commands;

public enum LinkFlightsResult { Success, ArrivalNotFound, DepartureNotFound, ArrivalAlreadyLinked, DepartureAlreadyLinked }

public sealed record LinkFlightsCommand(long ArrivalId, long DepartureId);

// Manual override of FlightAutoLinker's same-registration/turnaround-timing match — lets an
// operator pair two specific flights directly when the heuristic doesn't apply (no registration
// set yet, an unusual rotation, etc.). Both sides must currently be unlinked; relinking an
// already-linked flight requires an explicit Unlink first, same as the automatic linker's own rule.
public sealed class LinkFlightsCommandHandler(IAeroFlightDbContext db) : ICommandHandler<LinkFlightsCommand, LinkFlightsResult>
{
    public async Task<LinkFlightsResult> HandleAsync(LinkFlightsCommand command, CancellationToken ct)
    {
        var arrival = await db.Query<ArrivalFlight>().FirstOrDefaultAsync(f => f.Id == command.ArrivalId, ct);
        if (arrival is null)
            return LinkFlightsResult.ArrivalNotFound;

        var departure = await db.Query<DepartureFlight>().FirstOrDefaultAsync(f => f.Id == command.DepartureId, ct);
        if (departure is null)
            return LinkFlightsResult.DepartureNotFound;

        if (arrival.DepartureFlightId is not null)
            return LinkFlightsResult.ArrivalAlreadyLinked;
        if (departure.ArrivalFlightId is not null)
            return LinkFlightsResult.DepartureAlreadyLinked;

        arrival.LinkDepartureFlight(departure.Id);
        departure.LinkArrivalFlight(arrival.Id);
        await db.SaveChangesAsync(ct);
        return LinkFlightsResult.Success;
    }
}
