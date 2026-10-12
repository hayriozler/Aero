using AeroFlight.Application.Flights.Commands;
using AeroFlight.Application.Tenancy;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AeroFlight.Infrastructure.Scheduling;

[DisallowConcurrentExecution]
internal sealed class SeasonalFlightScheduleProcessorJob(
    IServiceScopeFactory scopeFactory, ITenantStore tenantStore, TimeProvider clock, ILogger<SeasonalFlightScheduleProcessorJob> logger)
    : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("tenantId")!;
        return TenantQuartzRunner.RunForTenantAsync(
            scopeFactory, tenantStore, logger, "Seasonal flight schedule processor", tenantId, ProcessForTenantAsync, context.CancellationToken);
    }

    private async Task ProcessForTenantAsync(IServiceProvider sp, TenantInfo tenant, CancellationToken ct)
    {
        var db = sp.GetRequiredService<AeroFlightDbContext>();
        var creationService = sp.GetRequiredService<IFlightCreationService>();

        var targetDate = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime.Date.AddDays(1));
        var targetDayOfWeek = ((int)new DateTime(targetDate.Year, targetDate.Month, targetDate.Day).DayOfWeek + 6) % 7 + 1;
        var targetStart = new DateTimeOffset(targetDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var targetEnd = targetStart.AddDays(1);

        var arrivalsCreated = await CreateArrivalsAsync(db, creationService, targetDate, targetDayOfWeek, targetStart, targetEnd, ct);
        var departuresCreated = await CreateDeparturesAsync(db, creationService, targetDate, targetDayOfWeek, targetStart, targetEnd, ct);

        logger.LogInformation(
            "Seasonal flight schedule processor created {ArrivalCount} arrival(s) and {DepartureCount} departure(s) for tenant {TenantId} on {Date}.",
            arrivalsCreated, departuresCreated, tenant.Id, targetDate);
    }

    private static async Task<int> CreateArrivalsAsync(
        AeroFlightDbContext db, IFlightCreationService creationService, DateOnly targetDate, int targetDayOfWeek,
        DateTimeOffset targetStart, DateTimeOffset targetEnd, CancellationToken ct)
    {
        var candidates = await db.SeasonalFlightScheduleArrivalFlightSet
            .Include(f => f.Routings)
            .Include(f => f.Season)
            .Where(f => f.PeriodBegin < targetEnd && f.PeriodEnd >= targetStart
                && f.Season!.BeginDate < targetEnd && f.Season.EndDate >= targetStart)
            .ToListAsync(ct);

        var created = 0;
        foreach (var candidate in candidates)
        {
            if (!OperatesOn(candidate.OpsDays, targetDayOfWeek))
                continue;

            var time = OnTargetDate(candidate.Sta, targetDate);
            var input = new ArrivalFlightCreationInput(
                candidate.AirlineCode, candidate.FlightNumber, time, candidate.OriginAirportCode,
                ExtraRoutings: [.. candidate.Routings.Where(r => r.LegNumber > 0).OrderBy(r => r.LegNumber).Select(r => (r.AirportCode, r.LegNumber))],
                CallSign: candidate.CallSign,
                AircraftTypeCode: candidate.AircraftTypeCode,
                Comment: candidate.Comment);

            var (outcome, _) = await creationService.CreateArrivalAsync(input, ct);
            if (outcome == CreateFlightOutcome.Created) created++;
        }

        return created;
    }

    private static async Task<int> CreateDeparturesAsync(
        AeroFlightDbContext db, IFlightCreationService creationService, DateOnly targetDate, int targetDayOfWeek,
        DateTimeOffset targetStart, DateTimeOffset targetEnd, CancellationToken ct)
    {
        var candidates = await db.SeasonalFlightScheduleDepartureFlightSet
            .Include(f => f.Routings)
            .Include(f => f.Season)
            .Where(f => f.PeriodBegin < targetEnd && f.PeriodEnd >= targetStart
                && f.Season!.BeginDate < targetEnd && f.Season.EndDate >= targetStart)
            .ToListAsync(ct);

        var created = 0;
        foreach (var candidate in candidates)
        {
            if (!OperatesOn(candidate.OpsDays, targetDayOfWeek))
                continue;

            var time = OnTargetDate(candidate.Std, targetDate);
            var input = new DepartureFlightCreationInput(
                candidate.AirlineCode, candidate.FlightNumber, time, candidate.DestinationAirportCode,
                ExtraRoutings: [.. candidate.Routings.Where(r => r.LegNumber > 0).OrderBy(r => r.LegNumber).Select(r => (r.AirportCode, r.LegNumber))],
                CallSign: candidate.CallSign,
                AircraftTypeCode: candidate.AircraftTypeCode,
                Comment: candidate.Comment);

            var (outcome, _) = await creationService.CreateDepartureAsync(input, ct);
            if (outcome == CreateFlightOutcome.Created) created++;
        }

        return created;
    }
    private static DateTimeOffset OnTargetDate(DateTimeOffset scheduled, DateOnly targetDate) =>
        new(targetDate.Year, targetDate.Month, targetDate.Day, scheduled.Hour, scheduled.Minute, scheduled.Second, scheduled.Offset);

    private static bool OperatesOn(string? opsDays, int isoDayOfWeek) =>
        opsDays is { Length: 7 } && opsDays[isoDayOfWeek - 1] == (char)('0' + isoDayOfWeek);
}
