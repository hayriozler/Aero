using AeroFlight.Application.Tenancy;
using AeroFlight.Contracts.Topology;
using AeroFlight.Infrastructure.Messaging;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text;

namespace AeroFlight.Infrastructure.Scheduling;

// Quartz rewrite of OutboxDispatcherService — same claim/publish logic, one tenant per firing
// (tenant id comes from the trigger's JobDataMap, see TenantJobRegistrar), publisher opened per firing.
[DisallowConcurrentExecution]
internal sealed class OutboxDispatchJob(
    IServiceScopeFactory scopeFactory, IMessageBus messageBus, ITenantStore tenantStore,
    TimeProvider clock, ILogger<OutboxDispatchJob> logger)
    : IJob
{
    private const int _batchSize = 20;

    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;
        var tenantId = context.MergedJobDataMap.GetString("tenantId")!;

        await using var publisher = await messageBus.CreatePublisherAsync(ct);
        await publisher.DeclareExchangeAsync(ExchangeNames.Metadata, ct);
        await publisher.DeclareExchangeAsync(ExchangeNames.Flights, ct);

        await TenantQuartzRunner.RunForTenantAsync(
            scopeFactory, tenantStore, logger, "Outbox dispatch (Quartz)", tenantId,
            (sp, tenant, tct) => DispatchTenantBatchAsync(sp, publisher, tenant, tct), ct);
    }

    private async Task DispatchTenantBatchAsync(IServiceProvider sp, IMessagePublisher publisher, TenantInfo tenant, CancellationToken ct)
    {
        var db = sp.GetRequiredService<AeroFlightDbContext>();

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var claimedIds = await TenantQuartzRunner.ClaimPendingIdsAsync(db, PendingClaimTarget.OutboxMessage, _batchSize, ct);
        if (claimedIds.Count == 0)
        {
            await tx.CommitAsync(ct);
            return;
        }

        var pending = await db.OutboxMessageSet
            .Where(m => claimedIds.Contains(m.Id))
            .ToListAsync(ct);
        var published = 0;

        foreach (var message in pending)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message.Payload);
                var headers = new Dictionary<string, object?> { [MessageHeaders.Tenant] = tenant.Id };
                await publisher.PublishAsync(message.Exchange, $"{tenant.Id}.{message.RoutingKey}", body, headers, ct);
                message.MarkPublished(clock.GetUtcNow());
                published++;
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
                logger.LogWarning(ex, "Failed to publish outbox message {Id} for tenant {TenantId}.", message.Id, tenant.Id);
            }
        }

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        logger.LogInformation("Outbox dispatch published {Published}/{Claimed} messages for tenant {TenantId}.",
            published, claimedIds.Count, tenant.Id);
    }
}
