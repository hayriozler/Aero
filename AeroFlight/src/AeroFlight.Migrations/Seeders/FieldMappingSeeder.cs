using AeroFids.Infrastructure.Flights;
using AeroFids.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeroFlight.Migrations.Seeders;

internal sealed partial class FieldMappingSeeder(FidsDbContext db, ILogger<FieldMappingSeeder> logger)
{
    private static readonly (string TargetField, string? FlightType, string SourcePath)[] _defaults =
    [
        (FidsFlightFields.FlightId, null, "flight.id"),
        (FidsFlightFields.FlightType, null, "flightType"),
        (FidsFlightFields.DesignatorAirlineCodes, null, "flight.airlineCode"),
        (FidsFlightFields.DesignatorFlightNumbers, null, "flight.flightNumber"),
        (FidsFlightFields.FlightDate, null, "flight.flightDate"),
        (FidsFlightFields.Status, null, "flight.status"),
        (FidsFlightFields.ScheduledTime, "Arrival", "flight.sta"),
        (FidsFlightFields.ScheduledTime, "Departure", "flight.std"),
        (FidsFlightFields.EstimatedTime, "Arrival", "flight.eta"),
        (FidsFlightFields.EstimatedTime, "Departure", "flight.etd"),
        (FidsFlightFields.TerminalCode, null, "flight.terminalCode"),
        (FidsFlightFields.RouteAirportCode, "Arrival", "flight.originAirportCode"),
        (FidsFlightFields.RouteAirportCode, "Departure", "flight.destinationAirportCode"),
        (FidsFlightFields.TenantTimeZone, null, "tenantTimeZone"),

        (FidsFlightFields.Gates, null, "gatesResources[].resource.code"),
        (FidsFlightFields.GateStatuses, null, "gatesResources[].resource.statusCode"),
        (FidsFlightFields.Belts, null, "beltsResources[].resource.code"),
        (FidsFlightFields.BeltStatuses, null, "beltsResources[].resource.statusCode"),
        (FidsFlightFields.Counters, "Departure", "countersResources[].resource.code"),
        (FidsFlightFields.CounterStatuses, "Departure", "countersResources[].resource.statusCode"),
        (FidsFlightFields.CounterDisplayCodes, "Departure", "countersResources[].resource.displayCode"),
        (FidsFlightFields.Spots, null, "parkPositionsResources[].resource.code"),
        (FidsFlightFields.SpotStatuses, null, "parkPositionsResources[].resource.flightStatusCode"),
    ];

    private static readonly (string TargetField, string? FlightType, string OldSourcePath)[] _corrections =
    [
        (FidsFlightFields.Gates, null, "flight.gates"),
        (FidsFlightFields.GateStatuses, null, "flight.gateStatusCode"),
        (FidsFlightFields.Belts, null, "flight.belts"),
        (FidsFlightFields.BeltStatuses, null, "flight.beltStatusCode"),
        (FidsFlightFields.Counters, "Departure", "flight.counters"),
        (FidsFlightFields.CounterStatuses, "Departure", "flight.counterStatusCode"),
        (FidsFlightFields.CounterDisplayCodes, "Departure", "flight.counterDisplayCode"),
    ];

    public async Task SeedAsync(CancellationToken ct)
    {
        var existingRows = await db.FieldMappingSet.ToListAsync(ct);
        var existingKeys = existingRows.Select(m => (m.TargetField, m.FlightType)).ToHashSet();

        var added = 0;
        foreach (var (targetField, flightType, sourcePath) in _defaults)
        {
            if (existingKeys.Contains((targetField, flightType)))
                continue;

            db.FieldMappingSet.Add(FieldMapping.Create(targetField, flightType, sourcePath));
            added++;
        }

        var corrected = 0;
        var defaultsByKey = _defaults.ToDictionary(d => (d.TargetField, d.FlightType), d => d.SourcePath);
        foreach (var (targetField, flightType, oldSourcePath) in _corrections)
        {
            var row = existingRows.FirstOrDefault(m => m.TargetField == targetField && m.FlightType == flightType);
            if (row is null || row.SourcePath != oldSourcePath)
                continue;

            row.Update(defaultsByKey[(targetField, flightType)]);
            corrected++;
        }

        if (added == 0 && corrected == 0)
            return;

        await db.SaveChangesAsync(ct);
        LogSeeded(logger, added, corrected);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Seeded {Added} new and corrected {Corrected} default AeroFlight field mappings.")]
    private static partial void LogSeeded(ILogger logger, int added, int corrected);
}
