using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace AeroFlight.Infrastructure.Scheduling;

public static class SchedulingServiceCollectionExtensions
{
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services)
    {
        services.AddQuartz();
        services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

        // Reads each tenant's own scheduling.JobSchedule table and registers one trigger per (job, tenant).
        services.AddHostedService<TenantJobRegistrar>();

        return services;
    }
}
