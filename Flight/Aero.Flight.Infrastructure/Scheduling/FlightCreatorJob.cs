using Aero.Flights.Application.Tenancy;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Aero.Flights.Infrastructure.Scheduling;

// Proof-of-concept nightly job: proves the Quartz pipeline fires and reaches every tenant DB.
[DisallowConcurrentExecution]
internal sealed class FlightCreatorJob(
    IServiceScopeFactory scopeFactory, ITenantStore tenantStore, TimeProvider clock, ILogger<FlightCreatorJob> logger)
    : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("tenantId")!;
        return TenantQuartzRunner.RunForTenantAsync(
            scopeFactory, tenantStore, logger, "Flight creator", tenantId, CreateForTenantAsync, context.CancellationToken);
    }

    private async Task CreateForTenantAsync(IServiceProvider sp, TenantInfo tenant, CancellationToken ct)
    {
        var db = sp.GetRequiredService<AeroFlightDbContext>();

        var airlines = await db.AirlineSet.ToListAsync(ct);
        var airports = await db.AirportSet.ToListAsync(ct);
        if (airlines.Count == 0 || airports.Count == 0)
        {
            logger.LogInformation("Flight creator skipped tenant {TenantId}: no airline/airport Metadata data.", tenant.Id);
            return;
        }

        var random = Random.Shared;
        var airline = airlines[random.Next(airlines.Count)];
        var airport = airports[random.Next(airports.Count)];
        var flightNumber = $"{airline.IataCode}{random.Next(100, 9999)}";
        var scheduled = clock.GetUtcNow().AddHours(random.Next(6, 18));

        Flight flight = random.Next(2) == 0
            ? ArrivalFlight.Create(airline.Id, flightNumber, scheduled, airport.Id)
            : DepartureFlight.Create(airline.Id, flightNumber, scheduled, airport.Id);

        db.Add(flight);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Flight creator created {FlightNumber} ({Direction}) for tenant {TenantId}.",
            flightNumber, flight is ArrivalFlight ? "Arrival" : "Departure", tenant.Id);
    }
}
