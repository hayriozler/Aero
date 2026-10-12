using AeroFlight.Domain.Configuration;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeroFlight.Migrations.Seeders;

internal sealed partial class AeroConfigSeeder(AeroFlightDbContext db, ILogger<AeroConfigSeeder> logger)
{
    public async Task SeedAsync(CancellationToken ct)
    {
        await EnsureAsync("Flights.DefaultPageSize", "200", "int", ct);

        // How long the Business Rules Engine caches the enabled-rules list per tenant before
        // re-reading it — rules change rarely, so this avoids a DB round trip on every flight
        // create/update/delete. 0 disables caching (always read fresh). See RuleEngine.
        await EnsureAsync("Rules.CacheSeconds", "30", "int", ct);

        // Airport-level display settings — editable per tenant, unlike the Tenant catalog row
        // (which only platform admins can reach and also holds the DB connection string).
        await EnsureAsync("Display.TimeZone", "Europe/Istanbul", "string", ct);
        await EnsureAsync("Display.DateFormat", "dd/MM/yyyy", "string", ct);
        await EnsureAsync("Display.TimeFormat", "HH:mm", "string", ct);
        // Plan/Actual columns in the flight detail Resources grids (Gates/Belts/Counters/Park
        // Position) are dense and don't need the year — a shorter, dedicated format from Display.DateFormat.
        await EnsureAsync("Resources.Grid.DateFormat", "dd.MM", "string", ct);
        // Duration/Offset applied by the business rules engine's RequiredGates/RequiredBelts/
        // RequiredCounters actions when auto-creating placeholder resources: the window opens
        // DefaultOffsetMinutes before the flight's own Sta/Std and stays open for
        // DefaultDurationMinutes. Status is not configured here — every resource-creation path
        // (this one included) gets its default status from IResourceDefaultStatusResolver, which
        // reads whichever code each kind's own status table (GateStatus/CounterStatus/
        // ArrivalBeltStatus/DepartureBeltStatus — see ResourceSeeder's *_statuses.csv) flags
        // IsDefault, so there's exactly one place that decides it. Gate is split by leg like belts —
        // ResourceTimeline.razor's unified gate browser (not leg-specific) still falls back to a
        // generic "gate" kind in FlightResourceEndpoints, mapped to the arrival figures below.
        await EnsureAsync("Resources.ArrivalGate.DefaultDurationMinutes", "60", "int", ct);
        await EnsureAsync("Resources.ArrivalGate.DefaultOffsetMinutes", "15", "int", ct);
        await EnsureAsync("Resources.DepartureGate.DefaultDurationMinutes", "25", "int", ct);
        await EnsureAsync("Resources.DepartureGate.DefaultOffsetMinutes", "5", "int", ct);
        await EnsureAsync("Resources.ArrivalBelt.DefaultDurationMinutes", "45", "int", ct);
        await EnsureAsync("Resources.ArrivalBelt.DefaultOffsetMinutes", "15", "int", ct);
        await EnsureAsync("Resources.DepartureBelt.DefaultDurationMinutes", "140", "int", ct);
        await EnsureAsync("Resources.DepartureBelt.DefaultOffsetMinutes", "20", "int", ct);
        await EnsureAsync("Resources.Counter.DefaultDurationMinutes", "120", "int", ct);
        await EnsureAsync("Resources.Counter.DefaultOffsetMinutes", "30", "int", ct);
        await EnsureAsync("Resources.ParkPosition.DefaultDurationMinutes", "120", "int", ct);
        await EnsureAsync("Resources.ParkPosition.DefaultOffsetMinutes", "15", "int", ct);

        // When a flight's status reaches StatusCode below, every gate/belt/counter allocation it
        // still has open is auto-closed (status set to that resource kind's own IsCloseValue status,
        // EndActual stamped now) — see ResourceAutoCloseService. "COM" (Completed) is the seeded
        // flight status meaning the flight has departed/arrived and its operation is fully done.
        await EnsureAsync("Resources.AutoCloseOnCompletion.Enabled", "true", "bool", ct);
        await EnsureAsync("Resources.AutoCloseOnCompletion.StatusCode", "COM", "string", ct);

        // Flight status chip color in the UI — key is FlightStatus.Color.{code}, value is a MudBlazor
        // Color name. FlightStatus.Color.Default is the fallback for any status code not listed here.
        await EnsureAsync("FlightStatus.Color.XLD", "Error", "color", ct);
        await EnsureAsync("FlightStatus.Color.DIV", "Error", "color", ct);
        await EnsureAsync("FlightStatus.Color.TMO", "Warning", "color", ct);
        await EnsureAsync("FlightStatus.Color.OFF", "Info", "color", ct);
        await EnsureAsync("FlightStatus.Color.TD", "Info", "color", ct);
        await EnsureAsync("FlightStatus.Color.ONB", "Info", "color", ct);
        await EnsureAsync("FlightStatus.Color.OFB", "Info", "color", ct);
        await EnsureAsync("FlightStatus.Color.Default", "Success", "color", ct);
    }

    private async Task EnsureAsync(string configurationName, string value, string configType, CancellationToken ct)
    {
        var exists = await db.AeroConfigurationSet.AnyAsync(c => c.ConfigurationName == configurationName, ct);
        if (exists) return;

        db.AeroConfigurationSet.Add(AeroConfiguration.Create(configurationName, value, configType));
        await db.SaveChangesAsync(ct);
        LogCreated(logger, configurationName);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Aero configuration '{ConfigurationName}' created.")]
    private static partial void LogCreated(ILogger logger, string configurationName);
}
