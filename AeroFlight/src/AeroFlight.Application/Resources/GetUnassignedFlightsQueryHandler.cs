using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Resources;

// Same split-then-concat-in-memory approach as GetGanttItemsQueryHandler — EF Core cannot
// translate a Concat/Union across the two independent flight queries.
public sealed class GetUnassignedFlightsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetUnassignedFlightsQuery, IReadOnlyList<UnassignedFlightItem>>
{
    // "Unassigned" must match GetGanttItemsQueryHandler's definition of an actual assignment (a row
    // with a non-null Code) — a resource row can exist with Code cleared (edited back to blank
    // rather than deleted outright), which must still count as unassigned here or the flight
    // disappears from both the Gantt bars AND the unassigned pool.
    public Task<IReadOnlyList<UnassignedFlightItem>> HandleAsync(GetUnassignedFlightsQuery query, CancellationToken ct)
        => query.ResourceType.ToLowerInvariant() switch
        {
            "gate" => GetBothAsync(f => f.OpsGates.Count(g => g.Code != null) == 0, f => f.OpsGates.Count(g => g.Code != null) == 0, query, ct),
            "counter" => GetDepartureOnlyAsync(f => f.OpsCounters.Count(c => c.Code != null) == 0, query, ct),
            "arrivalbelt" => GetArrivalOnlyAsync(f => f.OpsBelts.Count(b => b.Code != null) == 0, query, ct),
            "departurebelt" => GetDepartureOnlyAsync(f => f.OpsBelts.Count(b => b.Code != null) == 0, query, ct),
            "parkposition" => GetUnassignedParkPositionsAsync(query, ct),
            _ => Task.FromResult<IReadOnlyList<UnassignedFlightItem>>([]),
        };

    private async Task<IReadOnlyList<UnassignedFlightItem>> GetBothAsync(
        System.Linq.Expressions.Expression<Func<ArrivalFlight, bool>> arrivalUnassigned,
        System.Linq.Expressions.Expression<Func<DepartureFlight, bool>> departureUnassigned,
        GetUnassignedFlightsQuery query, CancellationToken ct)
    {
        var (from, to) = Range(query);
        var arrivals = await db.Query<ArrivalFlight>()
            .Where(f => f.Sta >= from && f.Sta <= to).Where(arrivalUnassigned)
            .Select(f => new UnassignedFlightItem(f.Id, "Arrival", f.FlightNumber, f.Sta))
            .ToListAsync(ct);
        var departures = await db.Query<DepartureFlight>()
            .Where(f => f.Std >= from && f.Std <= to).Where(departureUnassigned)
            .Select(f => new UnassignedFlightItem(f.Id, "Departure", f.FlightNumber, f.Std))
            .ToListAsync(ct);

        return [.. arrivals.Concat(departures).OrderBy(i => i.ScheduledTime)];
    }

    private async Task<IReadOnlyList<UnassignedFlightItem>> GetArrivalOnlyAsync(
        System.Linq.Expressions.Expression<Func<ArrivalFlight, bool>> unassigned,
        GetUnassignedFlightsQuery query, CancellationToken ct)
    {
        var (from, to) = Range(query);
        var items = await db.Query<ArrivalFlight>()
            .Where(f => f.Sta >= from && f.Sta <= to).Where(unassigned)
            .OrderBy(f => f.Sta)
            .Select(f => new UnassignedFlightItem(f.Id, "Arrival", f.FlightNumber, f.Sta))
            .ToListAsync(ct);

        return items;
    }

    private async Task<IReadOnlyList<UnassignedFlightItem>> GetDepartureOnlyAsync(
        System.Linq.Expressions.Expression<Func<DepartureFlight, bool>> unassigned,
        GetUnassignedFlightsQuery query, CancellationToken ct)
    {
        var (from, to) = Range(query);
        var items = await db.Query<DepartureFlight>()
            .Where(f => f.Std >= from && f.Std <= to).Where(unassigned)
            .OrderBy(f => f.Std)
            .Select(f => new UnassignedFlightItem(f.Id, "Departure", f.FlightNumber, f.Std))
            .ToListAsync(ct);

        return items;
    }

    private async Task<IReadOnlyList<UnassignedFlightItem>> GetUnassignedParkPositionsAsync(GetUnassignedFlightsQuery query, CancellationToken ct)
    {
        var (from, to) = Range(query);
        var arrivals = await db.Query<ArrivalFlight>()
            .Where(f => f.Sta >= from && f.Sta <= to)
            .Where(f => !db.Query<OpsFlightParkPosition>().Any(s => s.ArrivalFlightId == f.Id && s.Code != null))
            .Select(f => new UnassignedFlightItem(f.Id, "Arrival", f.FlightNumber, f.Sta))
            .ToListAsync(ct);
        var departures = await db.Query<DepartureFlight>()
            .Where(f => f.Std >= from && f.Std <= to)
            .Where(f => !db.Query<OpsFlightParkPosition>().Any(s => s.DepartureFlightId == f.Id && s.Code != null))
            .Select(f => new UnassignedFlightItem(f.Id, "Departure", f.FlightNumber, f.Std))
            .ToListAsync(ct);

        return [.. arrivals.Concat(departures).OrderBy(i => i.ScheduledTime)];
    }

    private static (DateTimeOffset From, DateTimeOffset To) Range(GetUnassignedFlightsQuery query) => (
        new DateTimeOffset(query.FromDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
        new DateTimeOffset(query.ToDate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero));
}
