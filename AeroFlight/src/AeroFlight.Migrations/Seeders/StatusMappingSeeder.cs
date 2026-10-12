using AeroFids.Infrastructure.Flights;
using AeroFids.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeroFlight.Migrations.Seeders;

// Maps AeroFlight's own status codes (see Seeds/flight_status.csv, gate_statuses.csv,
// counter_statuses.csv, arrivalbelt_statuses.csv, departurebelt_statuses.csv) onto AeroFids' internal
// vocabulary, so an AeroFlight-integrated tenant's board works out of the box. A standalone/
// non-AeroFlight tenant would instead define its own mapping via the admin API.
internal sealed partial class StatusMappingSeeder(FidsDbContext db, ILogger<StatusMappingSeeder> logger)
{
    private static readonly (string Dimension, string? FlightType, string ExternalValue, string InternalValue)[] _defaults =
    [
        // flight_status.csv — direction-agnostic codes.
        (StatusMappingDimensions.Flight, null, "SKD", InternalFlightStatuses.Scheduled),
        (StatusMappingDimensions.Flight, null, "TMO", InternalFlightStatuses.Scheduled),
        (StatusMappingDimensions.Flight, null, "OFB", InternalFlightStatuses.Departed),
        (StatusMappingDimensions.Flight, null, "OFF", InternalFlightStatuses.Departed),
        (StatusMappingDimensions.Flight, null, "TD", InternalFlightStatuses.Landed),
        (StatusMappingDimensions.Flight, null, "ONB", InternalFlightStatuses.Landed),
        (StatusMappingDimensions.Flight, null, "XLD", InternalFlightStatuses.Cancelled),
        (StatusMappingDimensions.Flight, null, "DIV", InternalFlightStatuses.Diverted),

        // flight_status.csv — "the whole process is over" codes mean a different internal status
        // depending on direction (departed vs. landed).
        (StatusMappingDimensions.Flight, "Departure", "COM", InternalFlightStatuses.Departed),
        (StatusMappingDimensions.Flight, "Departure", "OCC", InternalFlightStatuses.Departed),
        (StatusMappingDimensions.Flight, "Departure", "BIL", InternalFlightStatuses.Departed),
        (StatusMappingDimensions.Flight, "Arrival", "COM", InternalFlightStatuses.Landed),
        (StatusMappingDimensions.Flight, "Arrival", "OCC", InternalFlightStatuses.Landed),
        (StatusMappingDimensions.Flight, "Arrival", "BIL", InternalFlightStatuses.Landed),

        // gate_statuses.csv — only codes flagged is_open/is_close map to something other than Unknown.
        (StatusMappingDimensions.Gate, null, "O", InternalResourceStatuses.Open),
        (StatusMappingDimensions.Gate, null, "C", InternalResourceStatuses.Closed),

        // counter_statuses.csv
        (StatusMappingDimensions.Counter, null, "O", InternalResourceStatuses.Open),
        (StatusMappingDimensions.Counter, null, "C", InternalResourceStatuses.Closed),

        // arrivalbelt_statuses.csv / departurebelt_statuses.csv — share the same codes/flags for
        // A/S/C/X, so one "Belt" dimension covers both without ambiguity.
        (StatusMappingDimensions.Belt, null, "S", InternalResourceStatuses.Open),
        (StatusMappingDimensions.Belt, null, "C", InternalResourceStatuses.Closed),
    ];

    public async Task SeedAsync(CancellationToken ct)
    {
        // Per-row rather than "table empty" — mirrors FieldMappingSeeder — so a default added later
        // still reaches tenants already seeded by an earlier version of this list, without touching
        // any row a tenant has since customized via the admin API.
        var existing = await db.StatusMappingSet
            .Select(m => new { m.Dimension, m.FlightType, m.ExternalValue })
            .ToListAsync(ct);
        var existingKeys = existing.Select(m => (m.Dimension, m.FlightType, m.ExternalValue)).ToHashSet();

        var added = 0;
        foreach (var (dimension, flightType, externalValue, internalValue) in _defaults)
        {
            if (existingKeys.Contains((dimension, flightType, externalValue)))
                continue;

            db.StatusMappingSet.Add(StatusMapping.Create(dimension, flightType, externalValue, internalValue));
            added++;
        }

        if (added == 0)
            return;

        await db.SaveChangesAsync(ct);
        LogSeeded(logger, added);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Seeded {Count} default AeroFlight status mappings.")]
    private static partial void LogSeeded(ILogger logger, int count);
}
