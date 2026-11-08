using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Aero.Flights.Application.Scheduling;

public static class SchedulingServiceCollectionExtensions
{
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services)
    {
        services.AddQuartz();
        services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

        services.AddHostedService<TenantJobRegistrar>();

        return services;
    }
}
