using Aero.Flights.Application.Interfaces;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Routing;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Routings;

// Adding/editing/deleting a routing leg changes the flight's routing chain, so its CountryType
// (worst-wins across all legs) needs to be recomputed too — not just on the main flight save.
public interface IRoutingCountryTypeUpdater<TRouting> where TRouting : OpsRouting
{
    Task RefreshAsync(long flightId, CancellationToken ct);
}

public sealed class ArrivalRoutingCountryTypeUpdater(IAeroFlightDbContext db, IFlightCountryTypeResolver resolver)
    : IRoutingCountryTypeUpdater<OpsArrivalRouting>
{
    public async Task RefreshAsync(long flightId, CancellationToken ct)
    {
        var flight = await db.Query<ArrivalFlight>().Include(f => f.Routings).FirstOrDefaultAsync(f => f.Id == flightId, ct);
        if (flight is null)
            return;

        flight.UpdateCountryType(await resolver.ResolveAsync(flight.Routings.Select(r => r.Code), ct));
        await db.SaveChangesAsync(ct);
    }
}

public sealed class DepartureRoutingCountryTypeUpdater(IAeroFlightDbContext db, IFlightCountryTypeResolver resolver)
    : IRoutingCountryTypeUpdater<OpsDepartureRouting>
{
    public async Task RefreshAsync(long flightId, CancellationToken ct)
    {
        var flight = await db.Query<DepartureFlight>().Include(f => f.Routings).FirstOrDefaultAsync(f => f.Id == flightId, ct);
        if (flight is null)
            return;

        flight.UpdateCountryType(await resolver.ResolveAsync(flight.Routings.Select(r => r.Code), ct));
        await db.SaveChangesAsync(ct);
    }
}
