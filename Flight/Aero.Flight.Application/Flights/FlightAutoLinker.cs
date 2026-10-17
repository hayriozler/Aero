using Aero.Flights.Application.Interfaces;
using Aero.Flights.Domain.Ops;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights;

public interface IFlightAutoLinker
{
    Task TryLinkArrivalAsync(ArrivalFlight flight, CancellationToken ct);
    Task TryLinkDepartureAsync(DepartureFlight flight, CancellationToken ct);
}

// Same tail number, real turnaround only — a departure that already left before this arrival
// landed is not a match. When several candidates qualify, the closest turnaround wins.
public sealed class FlightAutoLinker(IAeroFlightDbContext db) : IFlightAutoLinker
{
    public async Task TryLinkArrivalAsync(ArrivalFlight flight, CancellationToken ct)
    {
        if (flight.DepartureFlightId is not null || string.IsNullOrWhiteSpace(flight.AircraftCode))
            return;

        var match = await db.Query<DepartureFlight>()
            .Where(d => d.AircraftCode == flight.AircraftCode && d.ArrivalFlightId == null && d.Std > flight.Sta)
            .OrderBy(d => d.Std)
            .FirstOrDefaultAsync(ct);
        if (match is null)
            return;

        flight.LinkDepartureFlight(match.Id);
        match.LinkArrivalFlight(flight.Id);
        await db.SaveChangesAsync(ct);
    }

    public async Task TryLinkDepartureAsync(DepartureFlight flight, CancellationToken ct)
    {
        if (flight.ArrivalFlightId is not null || string.IsNullOrWhiteSpace(flight.AircraftCode))
            return;

        var match = await db.Query<ArrivalFlight>()
            .Where(a => a.AircraftCode == flight.AircraftCode && a.DepartureFlightId == null && a.Sta < flight.Std)
            .OrderByDescending(a => a.Sta)
            .FirstOrDefaultAsync(ct);
        if (match is null)
            return;

        flight.LinkArrivalFlight(match.Id);
        match.LinkDepartureFlight(flight.Id);
        await db.SaveChangesAsync(ct);
    }
}
