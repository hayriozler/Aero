using AeroFlight.Application.Tenancy;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AeroFlight.Infrastructure.Scheduling;

// Reads each tenant's own scheduling.JobSchedule rows (that tenant's DB) and registers one
// Quartz trigger per (job, tenant) pair, so each tenant can run a job on its own cadence.
internal sealed class TenantJobRegistrar(
    ISchedulerFactory schedulerFactory, IServiceScopeFactory scopeFactory, ITenantStore tenantStore,
    ILogger<TenantJobRegistrar> logger)
    : IHostedService
{
    private static readonly IReadOnlyDictionary<string, Type> _jobTypes = new Dictionary<string, Type>
    {
        [nameof(FlightCreatorJob)] = typeof(FlightCreatorJob),
        [nameof(OutboxDispatchJob)] = typeof(OutboxDispatchJob),
        [nameof(InboxProcessorJob)] = typeof(InboxProcessorJob),
        [nameof(MessaggeCleanupJob)] = typeof(MessaggeCleanupJob),
        [nameof(SeasonalFlightScheduleProcessorJob)] = typeof(SeasonalFlightScheduleProcessorJob),
    };

    public async Task StartAsync(CancellationToken ct)
    {
        var scheduler = await schedulerFactory.GetScheduler(ct);
        var tenants = await tenantStore.GetActiveAsync(ct);

        foreach (var tenant in tenants)
        {
            try
            {
                await RegisterTenantJobsAsync(scheduler, tenant, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to register scheduled jobs for tenant {TenantId}.", tenant.Id);
            }
        }

        logger.LogInformation("Registered scheduled jobs for {Count} tenant(s).", tenants.Count);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    private async Task RegisterTenantJobsAsync(IScheduler scheduler, TenantInfo tenant, CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ITenantContext>().Set(tenant);
        var db = scope.ServiceProvider.GetRequiredService<AeroFlightDbContext>();

        var schedules = await db.JobScheduleSet.Where(s => s.IsEnabled).ToListAsync(ct);

        foreach (var schedule in schedules)
        {
            if (!_jobTypes.TryGetValue(schedule.JobName, out var jobType))
            {
                logger.LogWarning("Unknown job {JobName} in tenant {TenantId}'s schedule; skipping.", schedule.JobName, tenant.Id);
                continue;
            }

            // One tenant's malformed CronExpression must not abort every other job for that
            // tenant — each schedule is registered independently so a bad row only drops itself.
            try
            {
                await RegisterScheduleAsync(scheduler, tenant, schedule, jobType, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to register job {JobName} for tenant {TenantId} (cron '{Cron}'); skipping.",
                    schedule.JobName, tenant.Id, schedule.CronExpression);
            }
        }
    }

    private static async Task RegisterScheduleAsync(
        IScheduler scheduler, TenantInfo tenant, Domain.Scheduling.JobSchedule schedule, Type jobType, CancellationToken ct)
    {
        var jobKey = new JobKey(schedule.JobName, tenant.Id);
        if (!await scheduler.CheckExists(jobKey, ct))
        {
            var jobDetail = JobBuilder.Create(jobType).WithIdentity(jobKey).StoreDurably().Build();
            await scheduler.AddJob(jobDetail, replace: false, ct);
        }

        var triggerKey = new TriggerKey($"{schedule.JobName}-Trigger", tenant.Id);
        if (await scheduler.CheckExists(triggerKey, ct))
            return;

        var triggerBuilder = TriggerBuilder.Create()
            .ForJob(jobKey)
            .WithIdentity(triggerKey)
            .UsingJobData("tenantId", tenant.Id);

        triggerBuilder = schedule.CronExpression is { } cron
            ? triggerBuilder.WithCronSchedule(cron)
            : triggerBuilder.WithSimpleSchedule(x => x.WithIntervalInSeconds(schedule.IntervalSeconds ?? 60).RepeatForever());

        await scheduler.ScheduleJob(triggerBuilder.Build(), ct);
    }
}
