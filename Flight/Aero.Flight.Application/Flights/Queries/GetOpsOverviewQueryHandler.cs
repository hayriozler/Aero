using Aero.Flights.Application.Common;
using Aero.Flights.Application.Flights;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Queries;

public sealed class GetOpsOverviewQueryHandler(IAeroFlightDbContext db) : IQueryHandler<GetOpsOverviewQuery, OpsOverview>
{
    private static readonly string[] _departedStatuses =
        [FlightStatusCodes.OffBlock, FlightStatusCodes.Departed, FlightStatusCodes.Cancelled];

    public async Task<OpsOverview> HandleAsync(GetOpsOverviewQuery query, CancellationToken ct)
    {
        var date = query.Date;

        var arrivals = await db.Query<ArrivalFlight>()
            .Where(a => a.FlightDate == date)
            .Select(a => new ArrivalRaw(
                a.Sta,
                a.OpsGates.Where(g => g.Code != null).Select(g => g.Code!).ToList(),
                a.OpsBelts.Where(b => b.Code != null).Select(b => b.Code!).ToList()))
            .ToListAsync(ct);

        var departures = await db.Query<DepartureFlight>()
            .Where(d => d.FlightDate == date)
            .Select(d => new DepartureRaw(
                d.Id, d.FlightNumber, d.Std, d.Etd, d.Status,
                d.Routings.Where(r => r.LegNumber == 0).Select(r => r.Code).FirstOrDefault() ?? "",
                d.OpsGates.Where(g => g.Code != null).Select(g => g.Code!).ToList(),
                d.OpsBelts.Where(b => b.Code != null).Select(b => b.Code!).ToList(),
                d.OpsCounters.Where(c => c.Code != null).Select(c => c.Code!).ToList()))
            .ToListAsync(ct);

        var parkPositionCodes = await db.Query<OpsFlightParkPosition>()
            .Where(p => p.Code != null && (
                (p.ArrivalFlight != null && p.ArrivalFlight.FlightDate == date) ||
                (p.DepartureFlight != null && p.DepartureFlight.FlightDate == date)))
            .Select(p => p.Code!)
            .Distinct()
            .ToListAsync(ct);

        var movementTimes = arrivals.Select(a => new OpsMovementTime(a.Sta, "Arrival"))
            .Concat(departures.Select(d => new OpsMovementTime(d.Std, "Departure")))
            .ToList();

        var resourceOccupancy = new List<OpsResourceOccupancy>
        {
            new("Gate", arrivals.SelectMany(a => a.Gates).Concat(departures.SelectMany(d => d.Gates)).Distinct().Count()),
            new("Counter", departures.SelectMany(d => d.Counters).Distinct().Count()),
            new("ArrivalBelt", arrivals.SelectMany(a => a.Belts).Distinct().Count()),
            new("DepartureBelt", departures.SelectMany(d => d.Belts).Distinct().Count()),
            new("ParkPosition", parkPositionCodes.Count),
        };

        var unallocatedGates = arrivals.Count(a => a.Gates.Count == 0) + departures.Count(d => d.Gates.Count == 0);

        var nextOffBlocks = departures
            .Where(d => !_departedStatuses.Contains(d.Status))
            .OrderBy(d => d.Etd ?? d.Std)
            .Take(6)
            .Select(d => new OpsUpcomingDeparture(
                d.Id, FlightCodeFormatter.Format(null, d.FlightNumber), d.DestinationAirportCode,
                d.Std, d.Etd, d.Gates.FirstOrDefault(), d.Status))
            .ToList();

        return new OpsOverview(arrivals.Count, departures.Count, unallocatedGates, movementTimes, resourceOccupancy, nextOffBlocks);
    }

    private sealed record ArrivalRaw(DateTimeOffset Sta, List<string> Gates, List<string> Belts);

    private sealed record DepartureRaw(
        long Id, string FlightNumber, DateTimeOffset Std, DateTimeOffset? Etd, string Status,
        string DestinationAirportCode, List<string> Gates, List<string> Belts, List<string> Counters);
}
