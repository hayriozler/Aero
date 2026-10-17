using AeroFlight.Domain.Scheduling;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeroFlight.Migrations.Seeders;

internal sealed partial class JobScheduleSeeder(AeroFlightDbContext db, ILogger<JobScheduleSeeder> logger)
{
    public async Task SeedAsync(CancellationToken ct)
    {
        await EnsureAsync("FlightCreatorJob", "0 0 2 * * ?", null, ct);
        await EnsureAsync("OutboxDispatchJob", null, 2, ct);
        await EnsureAsync("InboxProcessorJob", null, 2, ct);
        await EnsureAsync("MessaggeCleanupJob", null, 3600, ct);
        await EnsureAsync("SeasonalFlightScheduleProcessorJob", "0 30 1 * * ?", null, ct);
    }

    private async Task EnsureAsync(string jobName, string? cronExpression, int? intervalSeconds, CancellationToken ct)
    {
        var exists = await db.JobScheduleSet.AnyAsync(s => s.Id == jobName, ct);
        if (exists) return;

        db.JobScheduleSet.Add(JobSchedule.Create(jobName, cronExpression, intervalSeconds));
        await db.SaveChangesAsync(ct);
        LogCreated(logger, jobName);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Job schedule '{JobName}' created.")]
    private static partial void LogCreated(ILogger logger, string jobName);
}
