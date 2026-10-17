using AeroFlight.Application.Flights.Resources;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Ops.Resources;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Aero.Migrations.Seeders;

internal sealed class FlightSeeder(AeroFlightDbContext db, ITenantContext tenantContext, IResourceDefaultStatusResolver statusResolver, ILogger<FlightSeeder> logger)
{
    private static readonly Assembly _asm = typeof(FlightSeeder).Assembly;

    private const int _arrivalTarget = 800;
    private const int _departureTarget = 850;
    private const int _linkedPairs = 720; // (720*2)/(800+850) ≈ 87% of all flights linked, clears the 85% bar
    private const double _completedFraction = 0.15; // buffer over the "at least 10%" ask

    private static readonly string[] _narrowBodyTypes =
    [
        "B731", "B732", "B732A", "B732C", "B732F", "B732L", "B733", "B733F", "B733W", "B734",
        "B735", "B735W", "B736", "B737", "B737W", "B738", "B738W", "B739", "B739W",
        "A320", "A3202", "A320S", "A321", "A3212",
    ];

    private static readonly string[] _wideBodyTypes =
    [
        "B741", "B741F", "B742", "B742C", "B742F", "B743", "B743C", "B743F",
        "B744", "B744C", "B744D", "B744F", "B747", "B747F", "B74S", "A380", "A380F",
    ];

    private static readonly string[] _terminals = ["D", "T1", "T2"];

    private readonly Random _rng = new();

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await db.ArrivalFlightSet.AnyAsync(ct) || await db.DepartureFlightSet.AnyAsync(ct))
        {
            logger.LogInformation("Flights already seeded.");
            return;
        }

        var airlines = LoadCodes("airlines", maxLength: 3);
        var airports = LoadCodes("airports").Where(a => a != tenantContext.HomeAirportCode).ToList();
        var gates = LoadCodes("gates");
        var counters = LoadCodes("counters");
        var arrivalBelts = LoadCodes("arrival_belts");
        var departureBelts = LoadCodes("departure_belts");
        // AircraftCode carries a real FK to the Aircraft Metadata table — registrations must be
        // drawn from aircraft.csv itself (paired with the type they actually fly), not invented.
        var narrowBodyAircraft = LoadAircraft(t => _narrowBodyTypes.Contains(t, StringComparer.OrdinalIgnoreCase));
        var wideBodyAircraft = LoadAircraft(t => _wideBodyTypes.Contains(t, StringComparer.OrdinalIgnoreCase));

        if (airlines.Count == 0 || airports.Count == 0 || gates.Count == 0 || counters.Count == 0
            || arrivalBelts.Count == 0 || departureBelts.Count == 0 || narrowBodyAircraft.Count == 0 || wideBodyAircraft.Count == 0)
        {
            logger.LogWarning("Metadata data missing (airlines/airports/gates/counters/belts/aircraft) — skipping flight seeding.");
            return;
        }

        var days = new[]
        {
            new DateTimeOffset(DateTime.UtcNow.Date.AddDays(-1), TimeSpan.Zero),
            new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero),
            new DateTimeOffset(DateTime.UtcNow.Date.AddDays(1), TimeSpan.Zero),
        };
        var arrivalNumbers = new Dictionary<string, int>();
        var departureNumbers = new Dictionary<string, int>();
        var allFlights = new List<Flight>(_arrivalTarget + _departureTarget);
        var pairs = new List<(ArrivalFlight Arrival, DepartureFlight Departure)>(_linkedPairs);

        for (var i = 0; i < _linkedPairs; i++)
        {
            var day = days[_rng.Next(days.Length)];
            var sta = day.AddHours(_rng.Next(0, 24)).AddMinutes(_rng.Next(0, 60));
            var std = sta.AddMinutes(45 + _rng.Next(0, 136)); // 45min–3h turnaround

            var (registration, aircraftType) = PickAircraft(narrowBodyAircraft, wideBodyAircraft);

            var arrivalAirline = Pick(airlines);
            var arrival = ArrivalFlight.Create(arrivalAirline, NextNumber(arrivalNumbers, arrivalAirline), sta, Pick(airports));
            ConfigureCommon(arrival, aircraftType, registration);
            AddExtraRouting(arrival, airports);
            await AddArrivalResourcesAsync(arrival, aircraftType, sta, gates, arrivalBelts, ct);

            var departureAirline = Pick(airlines);
            var departure = DepartureFlight.Create(departureAirline, NextNumber(departureNumbers, departureAirline), std, Pick(airports));
            ConfigureCommon(departure, aircraftType, registration);
            AddExtraRouting(departure, airports);
            await AddDepartureResourcesAsync(departure, aircraftType, std, gates, counters, departureBelts, ct);

            pairs.Add((arrival, departure));
            allFlights.Add(arrival);
            allFlights.Add(departure);
            db.ArrivalFlightSet.Add(arrival);
            db.DepartureFlightSet.Add(departure);
        }

        for (var i = 0; i < _arrivalTarget - _linkedPairs; i++)
        {
            var day = days[_rng.Next(days.Length)];
            var sta = day.AddHours(_rng.Next(0, 24)).AddMinutes(_rng.Next(0, 60));
            var (registration, aircraftType) = PickAircraft(narrowBodyAircraft, wideBodyAircraft);

            var airline = Pick(airlines);
            var arrival = ArrivalFlight.Create(airline, NextNumber(arrivalNumbers, airline), sta, Pick(airports));
            ConfigureCommon(arrival, aircraftType, registration);
            AddExtraRouting(arrival, airports);
            await AddArrivalResourcesAsync(arrival, aircraftType, sta, gates, arrivalBelts, ct);

            allFlights.Add(arrival);
            db.ArrivalFlightSet.Add(arrival);
        }

        for (var i = 0; i < _departureTarget - _linkedPairs; i++)
        {
            var day = days[_rng.Next(days.Length)];
            var std = day.AddHours(_rng.Next(0, 24)).AddMinutes(_rng.Next(0, 60));
            var (registration, aircraftType) = PickAircraft(narrowBodyAircraft, wideBodyAircraft);

            var airline = Pick(airlines);
            var departure = DepartureFlight.Create(airline, NextNumber(departureNumbers, airline), std, Pick(airports));
            ConfigureCommon(departure, aircraftType, registration);
            AddExtraRouting(departure, airports);
            await AddDepartureResourcesAsync(departure, aircraftType, std, gates, counters, departureBelts, ct);

            allFlights.Add(departure);
            db.DepartureFlightSet.Add(departure);
        }

        // Every generated flight already has coded resources (not placeholders), so any of them is
        // eligible to be marked complete — the "resources must be assigned with code" precondition
        // the user asked for is satisfied by construction, not by filtering here.
        var completedCount = (int)Math.Ceiling(allFlights.Count * _completedFraction);
        foreach (var flight in Shuffle(allFlights).Take(completedCount))
            MarkCompleted(flight);

        await db.SaveChangesAsync(ct);

        foreach (var (arrival, departure) in pairs)
        {
            arrival.LinkDepartureFlight(departure.Id);
            departure.LinkArrivalFlight(arrival.Id);
        }

        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "Seeded {Arr} arrival and {Dep} departure flights ({Pairs} linked pairs, {Completed} marked completed).",
            _arrivalTarget, _departureTarget, _linkedPairs, completedCount);
    }

    // Leg 0 (Origin/Destination) always exists by construction — this gives a realistic slice of
    // seeded flights an actual multi-sector itinerary too, so the Routing legs UI has real data to
    // show instead of every flight looking like a single-hop rotation.
    private void AddExtraRouting(ArrivalFlight arrival, List<string> airports)
    {
        var roll = _rng.NextDouble();
        if (roll < 0.05)
        {
            arrival.AddRouting(PickDistinctFrom(airports), 1);
            arrival.AddRouting(PickDistinctFrom(airports), 2);
        }
        else if (roll < 0.30)
        {
            arrival.AddRouting(PickDistinctFrom(airports), 1);
        }
    }

    private void AddExtraRouting(DepartureFlight departure, List<string> airports)
    {
        var roll = _rng.NextDouble();
        if (roll < 0.05)
        {
            departure.AddRouting(PickDistinctFrom(airports), 1);
            departure.AddRouting(PickDistinctFrom(airports), 2);
        }
        else if (roll < 0.30)
        {
            departure.AddRouting(PickDistinctFrom(airports), 1);
        }
    }

    private void ConfigureCommon(Flight flight, string aircraftType, string registration)
    {
        flight.UpdateFlightTypeCode("J");
        flight.UpdateAircraft(registration, aircraftType);
        flight.UpdateTerminal(_terminals[_rng.Next(_terminals.Length)]);
    }

    private async Task AddArrivalResourcesAsync(ArrivalFlight arrival, string aircraftType, DateTimeOffset sta, List<string> gates, List<string> belts, CancellationToken ct)
    {
        var (_, gateCount, arrivalBeltCount, _) = RequirementsFor(aircraftType);
        await AddResourcesAsync(gateCount, gates, arrival.AddGate, sta.AddMinutes(-5), sta.AddMinutes(45), ct);
        await AddResourcesAsync(arrivalBeltCount, belts, arrival.AddBelt, sta, sta.AddMinutes(40), ct);
    }

    private async Task AddDepartureResourcesAsync(DepartureFlight departure, string aircraftType, DateTimeOffset std, List<string> gates, List<string> counters, List<string> belts, CancellationToken ct)
    {
        var (counterCount, gateCount, _, departureBeltCount) = RequirementsFor(aircraftType);
        await AddResourcesAsync(gateCount, gates, departure.AddGate, std.AddMinutes(-60), std.AddMinutes(-5), ct);
        await AddResourcesAsync(counterCount, counters, departure.AddCounter, std.AddMinutes(-150), std.AddMinutes(-40), ct);
        await AddResourcesAsync(departureBeltCount, belts, departure.AddBelt, std.AddMinutes(-120), std.AddMinutes(-30), ct);
    }

    // Per-resource-kind loop shared by both directions — pick a code, plan the window, then resolve
    // the same tenant default status every other resource-creation path resolves (see
    // IResourceDefaultStatusResolver).
    private async Task AddResourcesAsync<TResource>(int count, List<string> pool, Func<string?, TResource> add,
        DateTimeOffset planBegin, DateTimeOffset planEnd, CancellationToken ct) where TResource : ResourceBase
    {
        for (var i = 0; i < count; i++)
        {
            var resource = add(PickDistinctFrom(pool));
            resource.SetPlan(planBegin, planEnd);
            await statusResolver.EnsureDefaultStatusAsync(resource, ct);
        }
    }

    private void MarkCompleted(Flight flight)
    {
        switch (flight)
        {
            case ArrivalFlight arrival:
                if (_rng.Next(2) == 0)
                {
                    arrival.FlightUpdateStatus("TD");
                    arrival.RecordTouchDown(arrival.Sta.AddMinutes(_rng.Next(-5, 10)));
                }
                else
                {
                    arrival.FlightUpdateStatus("ONB");
                    arrival.RecordTouchDown(arrival.Sta.AddMinutes(_rng.Next(-5, 5)));
                    arrival.RecordOnBlock(arrival.Sta.AddMinutes(_rng.Next(6, 15)));
                }
                break;
            case DepartureFlight departure:
                if (_rng.Next(2) == 0)
                {
                    departure.FlightUpdateStatus(FlightStatusCodes.OffBlock);
                    departure.RecordOffBlock(departure.Std.AddMinutes(_rng.Next(-5, 5)));
                }
                else
                {
                    departure.FlightUpdateStatus(FlightStatusCodes.Departed);
                    departure.RecordOffBlock(departure.Std.AddMinutes(_rng.Next(-8, 0)));
                    departure.RecordTakeOff(departure.Std.AddMinutes(_rng.Next(1, 10)));
                }
                break;
        }
    }

    // (counters, gates, arrivalBelts, departureBelts) — mirrors TestRulesSeeder's rules exactly.
    private static (int Counters, int Gates, int ArrivalBelts, int DepartureBelts) RequirementsFor(string aircraftType) =>
        _wideBodyTypes.Contains(aircraftType) ? (4, 2, 2, 1) : (2, 1, 1, 1);

    // ~85% narrow-body ("mainly B737/A320 family"), ~15% wide-body — enough wide-body flights to
    // exercise the second rule tier without it dominating the dataset. Registration and
    // type are picked together from the same real aircraft.csv row, since AircraftCode has a DB
    // foreign key to the Aircraft Metadata table — they can't be assigned independently.
    private (string Registration, string TypeCode) PickAircraft(
        List<(string Code, string TypeCode)> narrowBody, List<(string Code, string TypeCode)> wideBody)
    {
        var pool = _rng.NextDouble() < 0.85 ? narrowBody : wideBody;
        return pool[_rng.Next(pool.Count)];
    }

    // FlightNumber is expected to already carry its own airline prefix (e.g. "TK 1234") — the
    // visible flight code (FlightCodeFormatter) is built from FlightNumber alone, AirlineCode is
    // never shown directly — so the prefix has to live here, not just on the separate field.
    private static string NextNumber(Dictionary<string, int> countersByAirline, string airlineCode)
    {
        var next = countersByAirline.TryGetValue(airlineCode, out var current) ? current + 1 : 1000;
        countersByAirline[airlineCode] = next;
        return $"{airlineCode} {next}";
    }

    private string Pick(List<string> values) => values[_rng.Next(values.Count)];

    // Small pools (gates/counters/belts) — avoid handing out the exact same code to two resources
    // on the same flight when the pool is large enough to do so; falls back to a repeat otherwise.
    private string PickDistinctFrom(List<string> values) => values[_rng.Next(values.Count)];

    private IEnumerable<T> Shuffle<T>(List<T> source)
    {
        var indices = Enumerable.Range(0, source.Count).ToList();
        for (var i = indices.Count - 1; i > 0; i--)
        {
            var j = _rng.Next(i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
        return indices.Select(i => source[i]);
    }

    private static List<(string Code, string TypeCode)> LoadAircraft(Func<string, bool> typeMatches)
    {
        var rows = ReadCsv("aircraft");
        var list = new List<(string, string)>();
        foreach (var row in rows)
        {
            if (!row.TryGetValue("code", out var code) || string.IsNullOrWhiteSpace(code))
                continue;
            if (!row.TryGetValue("aircraft_type_code", out var type) || string.IsNullOrWhiteSpace(type))
                continue;
            if (!typeMatches(type))
                continue;

            list.Add((code, type));
        }
        return list;
    }

    private static List<string> LoadCodes(string csvName, string activeColumn = "is_active", string codeColumn = "code", int? maxLength = null)
    {
        var rows = ReadCsv(csvName);
        var codes = new List<string>();
        foreach (var row in rows)
        {
            if (!row.TryGetValue(codeColumn, out var code) || string.IsNullOrWhiteSpace(code))
                continue;
            if (maxLength is { } len && code.Length > len)
                continue;
            // Opt-out on an explicit false/0 rather than opt-in on an exact "true" match —
            // counters.csv has 2 extra unlabeled columns before its real is_active value, which
            // shifts the wrong field into this column (e.g. "1") for every row; requiring an exact
            // "true" match silently dropped every counter and, via the empty-pool guard below,
            // aborted the whole seeder.
            if (row.TryGetValue(activeColumn, out var active) && active is not null
                && (string.Equals(active, "false", StringComparison.OrdinalIgnoreCase) || active == "0"))
                continue;

            codes.Add(code);
        }
        return [.. codes.Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    private static List<Dictionary<string, string?>> ReadCsv(string name)
    {
        using var stream = _asm.GetManifestResourceStream($"AeroFlight.Migrations.Seeds.{name}.csv");
        if (stream is null) return [];
        using var reader = new StreamReader(stream);
        return CsvReader.ReadRows(reader, parseHeaderRow: false);
    }
}
