using AeroFlight.Application.Common;
using AeroFlight.Application.Configuration;
using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Ops;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AeroFlight.Application.Flights.Queries;

// EF Core cannot translate a Concat/Union following a LEFT JOIN (join ... into ... DefaultIfEmpty) —
// it throws "Unable to translate set operation after client projection has been applied" regardless
// of how simple the projected columns are. Arrivals and departures are queried independently instead,
// with every filter (date range, direction, airline, search, terminal, country type, status) pushed
// down to SQL — a flight that matches on its own side but is linked to a partner that does NOT
// independently match (the "either side" OR semantics the filters have always had) is topped up with
// a small by-id lookup afterwards, so merged rows never lose their other leg. Only the final pairing,
// sort and paging happen in memory, over the already-filtered (small) result set — not the whole table.
public sealed class GetFlightsQueryHandler(IAeroFlightDbContext db, ITenantContext tenantContext, IAeroConfigService aeroConfig)
    : IQueryHandler<GetFlightsQuery, PagedResult<MergedFlightRow>>
{
    private const string _defaultPageSizeConfigName = "Flights.DefaultPageSize";
    private const int _fallbackPageSize = 200;
    private const int _maxPageSize = 1000;

    // Origin/Destination are no longer stored columns — they're the routing leg with LegNumber 0 —
    // so they're read here as an explicit correlated subquery (EF-translatable), not the entity's
    // computed property (which EF Core cannot translate to SQL inside a server-side projection).
    //
    // Gates/Belts/Counters are no longer a manually-typed summary field — they're computed here
    // from the actual resource assignments (OpsGates/OpsBelts/OpsCounters), so the grid can never
    // drift out of sync with what's really assigned the way the old free-text field could.
    private static readonly Expression<Func<ArrivalFlight, ArrivalRaw>> _toArrivalRaw = a => new ArrivalRaw(
        a.Id, a.DepartureFlightId, a.AirlineCode, a.FlightNumber, a.FlightDate, a.Status,
        a.Sta, a.Eta,
        a.Routings.Where(r => r.LegNumber == 0).Select(r => r.Code).FirstOrDefault() ?? "",
        a.OpsGates.Where(g => g.Code != null).Select(g => g.Code!).ToList(),
        a.TerminalCode, a.AircraftCode, a.AircraftTypeCode, a.CountryType,
        a.OpsBelts.Where(b => b.Code != null).Select(b => b.Code!).ToList(),
        a.ParkPositions.Where(p => p.Code != null).Select(p => p.Code!).ToList());

    private static readonly Expression<Func<DepartureFlight, DepartureRaw>> _toDepartureRaw = d => new DepartureRaw(
        d.Id, d.ArrivalFlightId, d.AirlineCode, d.FlightNumber, d.FlightDate, d.Status,
        d.Std, d.Etd,
        d.Routings.Where(r => r.LegNumber == 0).Select(r => r.Code).FirstOrDefault() ?? "",
        d.OpsGates.Where(g => g.Code != null).Select(g => g.Code!).ToList(),
        d.TerminalCode, d.AircraftCode, d.AircraftTypeCode, d.CountryType,
        d.OpsCounters.Where(c => c.Code != null).Select(c => c.Code!).ToList(),
        d.ParkPositions.Where(p => p.Code != null).Select(p => p.Code!).ToList());

    public async Task<PagedResult<MergedFlightRow>> HandleAsync(GetFlightsQuery query, CancellationToken ct)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is { } requested and > 0
            ? Math.Min(requested, _maxPageSize)
            : await ResolveDefaultPageSizeAsync(ct);

        var wantedStatuses = query.Statuses is { Count: > 0 } ? query.Statuses : null;
        var wantedTerminals = query.TerminalCodes is { Count: > 0 }
            ? query.TerminalCodes.Select(t => t.ToUpperInvariant()).ToList()
            : null;
        var wantedCountryTypes = query.CountryTypes is { Count: > 0 }
            ? query.CountryTypes.Select(c => c.ToUpperInvariant()).ToList()
            : null;

        var wantedDirections = query.Directions is { Count: > 0 } ? query.Directions : null;
        var includeArrivals = wantedDirections is null || wantedDirections.Contains("Arrival", StringComparer.OrdinalIgnoreCase);
        var includeDepartures = wantedDirections is null || wantedDirections.Contains("Departure", StringComparer.OrdinalIgnoreCase);
        var wantedFlightTypeCodes = query.FlightTypeCodes is { Count: > 0 } ? query.FlightTypeCodes : null;

        var arrivals = includeArrivals
            ? await FilterArrivals(db.Query<ArrivalFlight>(), query, wantedStatuses, wantedTerminals, wantedCountryTypes, wantedFlightTypeCodes).Select(_toArrivalRaw).ToListAsync(ct)
            : [];
        var departures = includeDepartures
            ? await FilterDepartures(db.Query<DepartureFlight>(), query, wantedStatuses, wantedTerminals, wantedCountryTypes, wantedFlightTypeCodes).Select(_toDepartureRaw).ToListAsync(ct)
            : [];

        await TopUpLinkedPartnersAsync(arrivals, departures, ct);

        var departureById = departures.ToDictionary(d => d.Id);
        var pairedDepartureIds = new HashSet<long>();

        var pairs = new List<PairedRow>(arrivals.Count + departures.Count);
        foreach (var a in arrivals)
        {
            var d = a.DepartureFlightId is { } depId && departureById.TryGetValue(depId, out var found) ? found : null;
            if (d is not null)
                pairedDepartureIds.Add(d.Id);
            pairs.Add(new PairedRow(a, d));
        }
        foreach (var d in departures.Where(d => !pairedDepartureIds.Contains(d.Id)))
            pairs.Add(new PairedRow(null, d));

        var ordered = pairs.OrderBy(p => p.Arrival?.Sta ?? p.Departure!.Std).ToList();

        var totalCount = ordered.Count;
        var homeAirportCode = tenantContext.HomeAirportCode;
        var items = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => Combine(p.Arrival, p.Departure, homeAirportCode))
            .ToList();

        return new PagedResult<MergedFlightRow>(items, page, pageSize, totalCount);
    }

    // A linked partner may not independently match the filters (e.g. search matches the arrival's
    // flight number but not the departure's) — fetch it by id anyway so the merged row stays complete.
    private async Task TopUpLinkedPartnersAsync(List<ArrivalRaw> arrivals, List<DepartureRaw> departures, CancellationToken ct)
    {
        var departureIds = departures.Select(d => d.Id).ToHashSet();
        var missingDepartureIds = arrivals
            .Where(a => a.DepartureFlightId is { } id && !departureIds.Contains(id))
            .Select(a => a.DepartureFlightId!.Value)
            .Distinct()
            .ToList();
        if (missingDepartureIds.Count > 0)
        {
            var topUp = await db.Query<DepartureFlight>()
                .Where(d => missingDepartureIds.Contains(d.Id))
                .Select(_toDepartureRaw)
                .ToListAsync(ct);
            departures.AddRange(topUp);
        }

        var arrivalIds = arrivals.Select(a => a.Id).ToHashSet();
        var missingArrivalIds = departures
            .Where(d => d.ArrivalFlightId is { } id && !arrivalIds.Contains(id))
            .Select(d => d.ArrivalFlightId!.Value)
            .Distinct()
            .ToList();
        if (missingArrivalIds.Count > 0)
        {
            var topUp = await db.Query<ArrivalFlight>()
                .Where(a => missingArrivalIds.Contains(a.Id))
                .Select(_toArrivalRaw)
                .ToListAsync(ct);
            arrivals.AddRange(topUp);
        }
    }

    // A departure's Origin and an arrival's Destination are never stored explicitly — in a
    // single-airport AODB they're always this tenant's own airport, so a single-sided row fills
    // in the missing side with the home airport code rather than leaving it blank.
    private static MergedFlightRow Combine(ArrivalRaw? a, DepartureRaw? d, string homeAirportCode) => new(
        a?.Id, d?.Id,
        FlightCodeFormatter.Format(a?.FlightNumber, d?.FlightNumber),
        a?.AirlineCode, a?.FlightNumber, d?.AirlineCode, d?.FlightNumber,
        a?.FlightDate ?? d!.FlightDate,
        a?.Status, d?.Status,
        a?.Sta, a?.Eta, d?.Std, d?.Etd,
        a?.AircraftCode ?? d?.AircraftCode,
        a?.OriginAirportCode ?? homeAirportCode, d?.DestinationAirportCode ?? homeAirportCode,
        ResourceRangeFormatter.Format(Join(a?.Gates)), ResourceRangeFormatter.Format(Join(d?.Gates)),
        a?.TerminalCode, d?.TerminalCode,
        a?.AircraftTypeCode, d?.AircraftTypeCode,
        ResourceRangeFormatter.Format(Join(a?.BaggageBelts)), ResourceRangeFormatter.Format(Join(d?.Counters)),
        ResourceRangeFormatter.Format(Join(a?.ParkPositions)), ResourceRangeFormatter.Format(Join(d?.ParkPositions)));

    private static string? Join(List<string>? codes) => codes is null || codes.Count == 0 ? null : string.Join(",", codes);

    private static IQueryable<ArrivalFlight> FilterArrivals(
        IQueryable<ArrivalFlight> source, GetFlightsQuery query, IReadOnlyList<string>? wantedStatuses,
        IReadOnlyList<string>? wantedTerminals, IReadOnlyList<string>? wantedCountryTypes, IReadOnlyList<string>? wantedFlightTypeCodes)
    {
        if (query.FromDate is { } from)
            source = source.Where(a => a.FlightDate >= from);

        if (query.ToDate is { } to)
            source = source.Where(a => a.FlightDate <= to);

        if (!string.IsNullOrWhiteSpace(query.AirlineCode))
        {
            var code = query.AirlineCode.ToUpperInvariant();
            source = source.Where(a => a.AirlineCode.Equals(code));
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToUpperInvariant();
            source = source.Where(a => a.FlightNumber.Contains(search));
        }

        if (wantedTerminals is not null)
            source = source.Where(a => a.TerminalCode != null && wantedTerminals.Contains(a.TerminalCode.ToUpper()));

        if (wantedCountryTypes is not null)
            source = source.Where(a => a.CountryType != null && wantedCountryTypes.Contains(a.CountryType.ToUpper()));

        if (wantedStatuses is not null)
            source = source.Where(a => wantedStatuses.Contains(a.Status));

        if (wantedFlightTypeCodes is not null)
            source = source.Where(a => wantedFlightTypeCodes.Contains(a.FlightTypeCode));

        return source;
    }

    private static IQueryable<DepartureFlight> FilterDepartures(
        IQueryable<DepartureFlight> source, GetFlightsQuery query, IReadOnlyList<string>? wantedStatuses,
        IReadOnlyList<string>? wantedTerminals, IReadOnlyList<string>? wantedCountryTypes, IReadOnlyList<string>? wantedFlightTypeCodes)
    {
        if (query.FromDate is { } from)
            source = source.Where(d => d.FlightDate >= from);

        if (query.ToDate is { } to)
            source = source.Where(d => d.FlightDate <= to);

        if (!string.IsNullOrWhiteSpace(query.AirlineCode))
        {
            var code = query.AirlineCode.ToUpperInvariant();
            source = source.Where(d => d.AirlineCode.Equals(code));
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToUpperInvariant();
            source = source.Where(d => d.FlightNumber.Contains(search));
        }

        if (wantedTerminals is not null)
            source = source.Where(d => d.TerminalCode != null && wantedTerminals.Contains(d.TerminalCode.ToUpper()));

        if (wantedCountryTypes is not null)
            source = source.Where(d => d.CountryType != null && wantedCountryTypes.Contains(d.CountryType.ToUpper()));

        if (wantedStatuses is not null)
            source = source.Where(d => wantedStatuses.Contains(d.Status));

        if (wantedFlightTypeCodes is not null)
            source = source.Where(d => wantedFlightTypeCodes.Contains(d.FlightTypeCode));

        return source;
    }

    private async Task<int> ResolveDefaultPageSizeAsync(CancellationToken ct)
    {
        var parsed = await aeroConfig.GetIntAsync(_defaultPageSizeConfigName, _fallbackPageSize, ct);
        return parsed > 0 ? parsed : _fallbackPageSize;
    }

    private sealed record ArrivalRaw(
        long Id, long? DepartureFlightId, string AirlineCode, string FlightNumber, DateOnly FlightDate,
        string Status, DateTimeOffset Sta, DateTimeOffset? Eta, string OriginAirportCode,
        List<string> Gates, string? TerminalCode, string? AircraftCode, string? AircraftTypeCode, string? CountryType, List<string> BaggageBelts,
        List<string> ParkPositions);

    private sealed record DepartureRaw(
        long Id, long? ArrivalFlightId, string AirlineCode, string FlightNumber, DateOnly FlightDate,
        string Status, DateTimeOffset Std, DateTimeOffset? Etd, string DestinationAirportCode,
        List<string> Gates, string? TerminalCode, string? AircraftCode, string? AircraftTypeCode, string? CountryType, List<string> Counters,
        List<string> ParkPositions);

    private sealed record PairedRow(ArrivalRaw? Arrival, DepartureRaw? Departure);
}
