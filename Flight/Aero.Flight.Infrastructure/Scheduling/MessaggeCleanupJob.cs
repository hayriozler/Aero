using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application.Tenancy;
using Aero.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Aero.Flights.Infrastructure.Scheduling;

[DisallowConcurrentExecution]
internal sealed class MessaggeCleanupJob(
    IServiceScopeFactory scopeFactory, ITenantStore tenantStore, TimeProvider clock,
    ILogger<MessaggeCleanupJob> logger)
    : IJob
{
    private static readonly TimeSpan _retention = TimeSpan.FromDays(7);

    public Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("tenantId")!;
        return TenantQuartzRunner.RunForTenantAsync(
            scopeFactory, tenantStore, logger, "Messaging cleanup (Quartz)", tenantId, CleanupTenantAsync, context.CancellationToken);
    }

    private async Task CleanupTenantAsync(IServiceProvider sp, TenantInfo tenant, CancellationToken ct)
    {
        var db = sp.GetRequiredService<AeroFlightDbContext>();
        var cutoff = clock.GetUtcNow() - _retention;

        var outbox = await db.OutboxMessageSet
            .Where(m => m.PublishedAt != null && m.PublishedAt < cutoff)
            .ExecuteDeleteAsync(ct);

        var inbox = await db.InboxMessageSet
            .Where(m => m.ProcessedAt != null && m.ProcessedAt < cutoff)
            .ExecuteDeleteAsync(ct);

        if (outbox > 0 || inbox > 0)
            logger.LogInformation("Cleanup removed {Outbox} outbox and {Inbox} inbox rows for tenant {TenantId}.", outbox, inbox, tenant.Id);
    }
}
