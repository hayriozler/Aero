using AeroFlight.Domain.Metadatas;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Aircraft = AeroFlight.Domain.Metadatas.Aircraft;

namespace AeroFlight.Migrations.Seeders;

internal sealed class ResourceSeeder(AeroFlightDbContext db, ILogger<ResourceSeeder> logger)
{
    private static readonly Assembly _asm = typeof(ResourceSeeder).Assembly;
    private const string _seedPrefix = "AeroFlight.Migrations.Seeds.";

    // Case-insensitive base-name → resource-name map so callers don't have to match casing.
    private static readonly IReadOnlyDictionary<string, string> _seedResources = _asm
        .GetManifestResourceNames()
        .Where(n => n.StartsWith(_seedPrefix, StringComparison.Ordinal) && n.EndsWith(".csv", StringComparison.Ordinal))
        .ToDictionary(n => n[_seedPrefix.Length..^4], n => n, StringComparer.OrdinalIgnoreCase);

    public async Task SeedAsync(CancellationToken ct)
    {
        await SeedGateTypesAsync(ct);
        await SeedCountryTypesAsync(ct);
        await SeedParkPositionTypesAsync(ct);
        await SeedCountriesAsync(ct);

        ////Operations
        await SeedDelayReasonsAsync(ct);
        await SeedRemarksAsync(ct);

        await SeedDeBoardingStatusesAsync(ct);
        await SeedFlightStatusesAsync(ct);
        await SeedFlightTypesAsync(ct);
        await SeedArrivalBeltStatusesAsync(ct);
        await SeedDepartureBeltStatusesAsync(ct);
        await SeedRegionsAsync(ct);
        await SeedGateStatusesAsync(ct);
        await SeedCounterStatusesAsync(ct);
        await SeedTimezonesAsync(ct);
        await SeedAircraftNoiseCategoriesAsync(ct);

        await SeedAirportsAsync(ct);
        await SeedAirlinesAsync(ct);
        await SeedTerminalsAsync(ct);
        await SeedGatesAsync(ct);
        await SeedArrivalBeltsAsync(ct);
        await SeedDepartureBeltsAsync(ct);
        await SeedCountersAsync(ct);
        await SeedParkingPositionsAsync(ct);


        await SeedAircraftSizeCategoriesAsync(ct);
        await SeedAircraftTypesAsync(ct);
        await SeedAircraftAsync(ct);

        await SeedCompanyTypesAsync(ct);
    }

    // ── Generic EF-entity seeding ─────────────────────────────────────────────

    private async Task SeedEntitiesAsync<TEntity>(
        string csvName, DbSet<TEntity> set, IQueryable<string> existingIds,
        Func<Dictionary<string, string?>, string, TEntity?> factory, string logLabel, CancellationToken ct)
        where TEntity : class
    {
        var rows = ReadCsv(csvName);
        if (rows.Count == 0) return;

        var existing = await existingIds.ToHashSetAsync(ct);
        var toAdd = new List<TEntity>();

        foreach (var r in rows)
        {
            var code = r["code"];
            if (code is null || existing.Contains(code)) continue;
            var entity = factory(r, code);
            if (entity is null) continue;
            toAdd.Add(entity);
        }

        if (toAdd.Count == 0) return;
        set.AddRange(toAdd);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} {Label}.", toAdd.Count, logLabel);
    }

    private Task SeedCompanyTypesAsync(CancellationToken ct) =>
        SeedEntitiesAsync("company_types", db.CompanyTypeSet, db.CompanyTypeSet.Select(a => a.Id),
            (r, code) => CompanyType.Create(code, r["description"] ?? code),
            "company types", ct);

    private Task SeedAircraftSizeCategoriesAsync(CancellationToken ct) =>
        SeedEntitiesAsync("aircraft_size_categories", db.AircraftSizeCategorySet, db.AircraftSizeCategorySet.Select(a => a.Id),
            (r, code) => AircraftSizeCategory.Create(code, r["description"] ?? code),
            "aircraft size categories", ct);

    private Task SeedAirportsAsync(CancellationToken ct) =>
        SeedEntitiesAsync("airports", db.AirportSet, db.AirportSet.Select(a => a.Id),
            (r, code) =>
            {
                var airport = Airport.Create(code, r["iata_code"], r["name"], r["displayName"]);
                airport.Update(r["iata_code"], r["icao_code"], r["name"], r["displayName"], r["country_code"], r["timezone"]);
                return airport;
            },
            "airports", ct);

    // Also builds AirlineContact side-rows (Sita address) per airline — doesn't fit the generic
    // one-entity-per-row helper, kept as its own method.
    private async Task SeedAirlinesAsync(CancellationToken ct)
    {
        var rows = ReadCsv("airlines");
        if (rows.Count == 0) return;

        var existing = await db.AirlineSet.Select(a => a.Id).ToHashSetAsync(ct);
        var toAdd = new List<Airline>();
        var contactsToAdd = new List<AirlineContact>();

        foreach (var r in rows)
        {
            var code = r["code"];
            if (code is null || existing.Contains(code)) continue;

            var airline = Airline.Create(code, r["iata_code"] ?? code, r["name"]);
            airline.Update(
                r["icao_code"], r["callsign"],
                r["name"], r["displayName"],
                r["parent_airline_code"], r["country_code"]);

            toAdd.Add(airline);

            if (!string.IsNullOrWhiteSpace(r["sita_address"]))
                contactsToAdd.Add(AirlineContact.Create(code, AirlineContactType.Sita, r["sita_address"]!));
        }

        if (toAdd.Count == 0) return;
        db.AirlineSet.AddRange(toAdd);
        db.AirlineContactSet.AddRange(contactsToAdd);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} airlines.", toAdd.Count);
    }

    private Task SeedTerminalsAsync(CancellationToken ct) =>
        SeedEntitiesAsync("terminals", db.TerminalSet, db.TerminalSet.Select(t => t.Id),
            (r, code) =>
            {
                var terminal = Terminal.Create(code);
                terminal.Update(r["description"]);
                return terminal;
            },
            "terminals", ct);

    private Task SeedGatesAsync(CancellationToken ct) =>
        SeedEntitiesAsync("gates", db.GateSet, db.GateSet.Select(g => g.Id),
            (r, code) =>
            {
                var gate = Gate.Create(code, r["terminal_code"], r["gate_type_code"]);
                gate.Update(
                    r["gate_type_code"], r["terminal_code"], r["description"], ParseInt(r["capacity"]),
                    ParseBool(r["has_loading_bridge"]), ParseBool(r["has_pca"]));
                return gate;
            },
            "gates", ct);

    private Task SeedArrivalBeltsAsync(CancellationToken ct) =>
        SeedEntitiesAsync("arrival_belts", db.ArrivalBeltSet, db.ArrivalBeltSet.Select(b => b.Id),
            (r, code) =>
            {
                var arrBelt = ArrivalBelt.Create(code, r["terminal_code"]);
                arrBelt.Update(r["terminal_code"], r["description"], ParseInt(r["capacity"]));
                return arrBelt;
            },
            "Arrival Belts", ct);

    private Task SeedDepartureBeltsAsync(CancellationToken ct) =>
        SeedEntitiesAsync("departure_belts", db.DepartureBeltSet, db.DepartureBeltSet.Select(b => b.Id),
            (r, code) =>
            {
                var depBelt = DepartureBelt.Create(code, r["terminal_code"]);
                depBelt.Update(r["terminal_code"], r["description"], ParseInt(r["capacity"]));
                return depBelt;
            },
            "Departure Belts", ct);

    private Task SeedCountersAsync(CancellationToken ct) =>
        SeedEntitiesAsync("counters", db.CounterSet, db.CounterSet.Select(c => c.Id),
            (r, code) =>
            {
                var counter = Counter.Create(code, r["terminal_code"]);
                counter.Update(r["terminal_code"], r["description"], ParseInt(r["capacity"]));
                return counter;
            },
            "counters", ct);

    private Task SeedParkingPositionsAsync(CancellationToken ct) =>
        SeedEntitiesAsync("parkingpositions", db.ParkingPositionSet, db.ParkingPositionSet.Select(s => s.Id),
            (r, code) =>
            {
                var parkPosition = ParkPosition.Create(code);
                parkPosition.Update(
                    r["parkingposition_type_code"], r["description"],
                    ParseInt(r["capacity"]),
                    ParseDecimal(r["latitude"]),
                    ParseDecimal(r["longitude"]),
                    ParseDecimal(r["angle"]));
                return parkPosition;
            },
            "parking positions", ct);

    private Task SeedCountriesAsync(CancellationToken ct) =>
        SeedEntitiesAsync("countries", db.CountrySet, db.CountrySet.Select(c => c.Id),
            (r, code) => Country.Create(code, r["name"]!, r["country_type_code"]!),
            "countries", ct);

    // Saves + logs failures per row instead of a single batched SaveChanges — doesn't fit the
    // generic all-or-nothing helper, kept as its own method.
    private async Task SeedAircraftTypesAsync(CancellationToken ct)
    {
        var rows = ReadCsv("aircraft_types");
        if (rows.Count == 0) return;

        var existing = await db.AircraftTypeSet.Select(a => a.Id).ToHashSetAsync(ct);
        var seeded = 0;

        foreach (var r in rows)
        {
            var code = r["code"];
            if (code is null || existing.Contains(code)) continue;

            var at = AircraftType.Create(code);
            at.Update(
                r["IataCode"],
                r["IataSubCode"],
                r["IcaoCode"],
                r["SizeCategory"],
                r.GetValueOrDefault("NoiseCategory"),
                r["Manufacturer"],
                r["Model"],
                ParseDecimal(r["Mtow"]),
                ParseDecimal(r["LandingWeight"]),
                ParseInt(r["SeatCapacity"]),
                ParseBool(r["IsBridgeRequired"]));

            db.AircraftTypeSet.Add(at);
            try
            {
                await db.SaveChangesAsync(ct);
                seeded++;
            }
            catch (Exception ex)
            {
                db.Entry(at).State = EntityState.Detached;
                logger.LogError(ex, "Failed to seed aircraft type '{Code}' — skipped.", code);
            }
        }

        logger.LogInformation("Seeded {Count} aircraft types.", seeded);
    }

    private Task SeedAircraftAsync(CancellationToken ct) =>
        SeedEntitiesAsync("aircraft", db.AircraftSet, db.AircraftSet.Select(a => a.Id),
            (r, code) =>
            {
                var aircraft = Aircraft.Create(code, r["aircraft_type_code"]!);
                aircraft.Update(r["airline_code"], r["owner"], r["size_category"], r["noise_category"],
                    ParseDecimal(r["bill_weight"]), ParseInt(r["seat_capacity"]));
                return aircraft;
            },
            "aircraft", ct);

    private Task SeedDelayReasonsAsync(CancellationToken ct) =>
        SeedEntitiesAsync("delayreasons", db.DelayReasonSet, db.DelayReasonSet.Select(s => s.Id),
            (r, code) => DelayReason.Create(code, r["alphabeticcode"], r["description"]),
            "delay reasons", ct);

    private Task SeedRemarksAsync(CancellationToken ct) =>
        SeedEntitiesAsync("remarks", db.RemarkSet, db.RemarkSet.Select(s => s.Id),
            (r, code) => Remark.Create(code, r["description"]!),
            "remarks", ct);

    // ── Raw-SQL seeding (entities with no public constructor/factory) ─────────
    //
    // GateType, ParkPositionType, CountryType, DeBoardingStatus, FlightStatus, FlightType, Region,
    // Timezone, AircraftNoiseCategory, GateStatus and CounterStatus are EF-materialization-only —
    // private constructor, no factory — so they can't be built in code without changing the Domain
    // entity, which is out of scope for this seeder refactor. ArrivalBeltStatus and
    // DepartureBeltStatus do have a public Create(code, description), but it only covers 2 of the
    // 5 columns seeded here (IsDefault/IsOpenValue/IsCloseValue have no way to be set from outside
    // the entity), so building them via Create would silently drop those column values — kept as
    // raw SQL too, for the same reason.

    private async Task SeedRawSqlAsync(
        string csvName, IQueryable<string> existingCodes, string logLabel,
        Action<Dictionary<string, string?>> insertRow, CancellationToken ct)
    {
        var rows = ReadCsv(csvName);
        if (rows.Count == 0) return;

        var existing = await existingCodes.ToHashSetAsync(ct);
        foreach (var r in rows)
        {
            var code = r["code"];
            if (code is null || existing.Contains(code)) continue;
            insertRow(r);
        }

        logger.LogInformation("Seeded {Count} {Label}.", rows.Count, logLabel);
    }

    private void InsertMetadataRow(string table, string[] columns, params object?[] values)
    {
        // Built as a plain (non-interpolated) string so EF1002 doesn't flag it — table/column
        // names are internal literals, actual values still go through the parameterized {n} slots.
        var columnList = string.Join(", ", columns.Select(c => "\"" + c + "\""));
        var placeholders = string.Join(", ", Enumerable.Range(0, values.Length).Select(i => "{" + i + "}"));
        var sql = "INSERT INTO Metadata.\"" + table + "\" (" + columnList + ") VALUES (" + placeholders + ")";
        db.Database.ExecuteSqlRaw(sql, values);
    }

    private Task SeedGateTypesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("gate_types", db.GateTypeSet.Select(g => g.Code), "gate types",
            r => InsertMetadataRow("GateType", ["Code", "Description"], r["code"], r["description"]), ct);

    private Task SeedParkPositionTypesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("parkposition_types", db.ParkPositionTypeSet.Select(g => g.Code), "Park Position types",
            r => InsertMetadataRow("ParkPositionType", ["Code", "Description"], r["code"], r["description"]), ct);

    private Task SeedCountryTypesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("country_types", db.CountryTypeSet.Select(c => c.Code), "country types",
            r => InsertMetadataRow("CountryType", ["Code", "Description", "Severity"],
                r["code"], r["description"], ParseInt(r["severity"])), ct);

    private Task SeedDeBoardingStatusesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("deboarding_statuses", db.DeBoardingStatusSet.Select(d => d.Code), "deboarding statuses",
            r => InsertMetadataRow("DeBoardingStatus", ["Code", "Description"], r["code"], r["description"]), ct);

    private Task SeedFlightStatusesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("flight_status", db.FlightStatusSet.Select(f => f.Code), "flight statuses",
            r => InsertMetadataRow("FlightStatus", ["Code", "Name", "Description", "IsUpdatable"],
                r["code"], r["name"], r["description"], ParseBool(r["is_updatable"])), ct);

    private Task SeedFlightTypesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("flight_types", db.FlightTypeSet.Select(f => f.Code), "flight types",
            r => InsertMetadataRow("FlightType", ["Code", "Description", "DisplayCode"],
                r["code"], r["description"], r["display_code"]), ct);

    private Task SeedArrivalBeltStatusesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("arrivalbelt_statuses", db.ArrivalBeltStatusSet.Select(b => b.Code), "arrival belt statuses",
            r => InsertMetadataRow("ArrivalBeltStatus", ["Code", "Description", "IsDefault", "IsOpenValue", "IsCloseValue"],
                r["code"], r["description"], ParseBool(r["is_default"]), ParseBool(r["is_open"]), ParseBool(r["is_close"])), ct);

    private Task SeedDepartureBeltStatusesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("departurebelt_statuses", db.DepartureBeltStatusSet.Select(b => b.Code), "departure belt statuses",
            r => InsertMetadataRow("DepartureBeltStatus", ["Code", "Description", "IsDefault", "IsOpenValue", "IsCloseValue"],
                r["code"], r["description"], ParseBool(r["is_default"]), ParseBool(r["is_open"]), ParseBool(r["is_close"])), ct);

    private Task SeedRegionsAsync(CancellationToken ct) =>
        SeedRawSqlAsync("regions", db.RegionSet.Select(r => r.Code), "regions",
            r => InsertMetadataRow("Region", ["Code", "Description"], r["code"], r["description"]), ct);

    private Task SeedTimezonesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("timezones", db.TimezoneSet.Select(t => t.Code), "time zones",
            r => InsertMetadataRow("Timezone", ["Code", "Description"], r["code"], r["description"] ?? r["code"]), ct);

    private Task SeedAircraftNoiseCategoriesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("aircraft_noise_categories", db.AircraftNoiseCategorySet.Select(n => n.Code), "aircraft noise categories",
            r => InsertMetadataRow("AircraftNoiseCategory", ["Code", "Description"], r["code"], r["description"] ?? r["code"]), ct);

    private Task SeedGateStatusesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("gate_statuses", db.GateStatusSet.Select(g => g.Code), "gate statuses",
            r => InsertMetadataRow("GateStatus", ["Code", "Description", "IsDefault", "IsOpenValue", "IsCloseValue"],
                r["code"], r["description"], ParseBool(r["is_default"]), ParseBool(r["is_open"]), ParseBool(r["is_close"])), ct);

    private Task SeedCounterStatusesAsync(CancellationToken ct) =>
        SeedRawSqlAsync("counter_statuses", db.CounterStatusSet.Select(c => c.Code), "counter statuses",
            r => InsertMetadataRow("CounterStatus", ["Code", "Description", "IsDefault", "IsOpenValue", "IsCloseValue"],
                r["code"], r["description"], ParseBool(r["is_default"]), ParseBool(r["is_open"]), ParseBool(r["is_close"])), ct);

    // ── CSV helpers ────────────────────────────────────────────────────────────

    private static List<Dictionary<string, string?>> ReadCsv(string name)
    {
        if (!_seedResources.TryGetValue(name, out var resourceName))
            throw new InvalidOperationException(
                $"Seed CSV '{name}' not found. Available: {string.Join(", ", _seedResources.Keys.Order())}.");

        using var stream = _asm.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return CsvReader.ReadRows(reader, parseHeaderRow: true);
    }

    private static bool ParseBool(string? value, bool defaultValue = false) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "true" or "1" or "yes" or "y" => true,
            "false" or "0" or "no" or "n" => false,
            _ => defaultValue
        };

    private static int? ParseInt(string? value) =>
        int.TryParse(value?.Trim(), out var v) ? v : null;

    private static decimal? ParseDecimal(string? value) =>
        decimal.TryParse(value?.Trim(),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var v) ? v : null;
}
