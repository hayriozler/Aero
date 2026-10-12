using AeroFlight.Application.Common;
using AeroFlight.Application.Flights;
using AeroFlight.Application.Interfaces;
using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Resources;

// Each branch below fetches arrivals/departures as two independent, flat (translatable) queries
// and concatenates them in memory rather than via IQueryable.Concat — EF Core cannot translate a
// Concat/Union once either side requires any client evaluation, regardless of how simple the
// projected columns are (see GetFlightsQueryHandler for the same fix, hit first).
public sealed class GetGanttItemsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetGanttItemsQuery, IReadOnlyList<GanttItem>>
{
    public Task<IReadOnlyList<GanttItem>> HandleAsync(GetGanttItemsQuery query, CancellationToken ct)
        => query.ResourceType.ToLowerInvariant() switch
        {
            "gate" => GetGateItemsAsync(query, ct),
            "counter" => GetCounterItemsAsync(query, ct),
            "arrivalbelt" => GetArrivalBeltItemsAsync(query, ct),
            "departurebelt" => GetDepartureBeltItemsAsync(query, ct),
            "parkposition" => GetParkPositionItemsAsync(query, ct),
            _ => Task.FromResult<IReadOnlyList<GanttItem>>([]),
        };

    private async Task<IReadOnlyList<GanttItem>> GetGateItemsAsync(GetGanttItemsQuery query, CancellationToken ct)
    {
        var arrivals = await db.Query<ArrivalFlight>().SelectMany(f => f.OpsGates, (f, g) => new RawItem(
            g.Id, f.Id, g.Code, g.BeginActual ?? g.BeginPlan, g.EndActual ?? g.EndPlan,
            f.AirlineCode, f.FlightNumber, null, null, "Arrival")).ToListAsync(ct);
        var departures = await db.Query<DepartureFlight>().SelectMany(f => f.OpsGates, (f, g) => new RawItem(
            g.Id, f.Id, g.Code, g.BeginActual ?? g.BeginPlan, g.EndActual ?? g.EndPlan,
            null, null, f.AirlineCode, f.FlightNumber, "Departure")).ToListAsync(ct);

        return Materialize("Gate", arrivals.Concat(departures), query);
    }

    private async Task<IReadOnlyList<GanttItem>> GetCounterItemsAsync(GetGanttItemsQuery query, CancellationToken ct)
    {
        var items = await db.Query<DepartureFlight>().SelectMany(f => f.OpsCounters, (f, c) => new RawItem(
            c.Id, f.Id, c.Code, c.BeginActual ?? c.BeginPlan, c.EndActual ?? c.EndPlan,
            null, null, f.AirlineCode, f.FlightNumber, "Departure")).ToListAsync(ct);

        return Materialize("Counter", items, query);
    }

    private async Task<IReadOnlyList<GanttItem>> GetArrivalBeltItemsAsync(GetGanttItemsQuery query, CancellationToken ct)
    {
        var items = await db.Query<ArrivalFlight>().SelectMany(f => f.OpsBelts, (f, b) => new RawItem(
            b.Id, f.Id, b.Code, b.BeginActual ?? b.BeginPlan, b.EndActual ?? b.EndPlan,
            f.AirlineCode, f.FlightNumber, null, null, "Arrival")).ToListAsync(ct);

        return Materialize("ArrivalBelt", items, query);
    }

    private async Task<IReadOnlyList<GanttItem>> GetDepartureBeltItemsAsync(GetGanttItemsQuery query, CancellationToken ct)
    {
        var items = await db.Query<DepartureFlight>().SelectMany(f => f.OpsBelts, (f, b) => new RawItem(
            b.Id, f.Id, b.Code, b.BeginActual ?? b.BeginPlan, b.EndActual ?? b.EndPlan,
            null, null, f.AirlineCode, f.FlightNumber, "Departure")).ToListAsync(ct);

        return Materialize("DepartureBelt", items, query);
    }

    private async Task<IReadOnlyList<GanttItem>> GetParkPositionItemsAsync(GetGanttItemsQuery query, CancellationToken ct)
    {
        // A turnaround row carries both leg ids — the arrival leg is used as the "owning" flight for
        // move/resize interactions since ResourceGantt only offers one flightId+direction per bar.
        var items = await db.Query<OpsFlightParkPosition>().Select(s => new RawItem(
            s.Id, (s.ArrivalFlightId ?? s.DepartureFlightId) ?? 0, s.Code, s.BeginActual ?? s.BeginPlan, s.EndActual ?? s.EndPlan,
            s.ArrivalFlight != null ? s.ArrivalFlight.AirlineCode : null,
            s.ArrivalFlight != null ? s.ArrivalFlight.FlightNumber : null,
            s.DepartureFlight != null ? s.DepartureFlight.AirlineCode : null,
            s.DepartureFlight != null ? s.DepartureFlight.FlightNumber : null,
            s.ArrivalFlightId != null && s.DepartureFlightId != null
                ? "Turnaround"
                : s.ArrivalFlightId != null ? "Arrival" : "Departure")).ToListAsync(ct);

        return Materialize("ParkPosition", items, query);
    }

    private static IReadOnlyList<GanttItem> Materialize(string resourceType, IEnumerable<RawItem> source, GetGanttItemsQuery query)
    {
        var rangeStart = new DateTimeOffset(query.FromDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var rangeEnd = new DateTimeOffset(query.ToDate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);


        return [.. source
            .Where(r => r.Code != null && r.Start != null && r.End != null && r.Start <= rangeEnd && r.End >= rangeStart)
            .Select(r => new GanttItem(
                r.ResourceId, r.FlightId, resourceType, r.Code!, r.Start!.Value, r.End!.Value,
                FlightCodeFormatter.Format(r.ArrivalFlightNumber, r.DepartureFlightNumber),
                r.Direction))];
    }

    private sealed record RawItem(
        long ResourceId,
        long FlightId,
        string? Code,
        DateTimeOffset? Start,
        DateTimeOffset? End,
        string? ArrivalAirlineCode,
        string? ArrivalFlightNumber,
        string? DepartureAirlineCode,
        string? DepartureFlightNumber,
        string Direction);
}
