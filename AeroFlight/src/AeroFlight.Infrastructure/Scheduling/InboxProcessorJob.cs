using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Tenancy;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AeroFlight.Infrastructure.Scheduling;

[DisallowConcurrentExecution]
internal sealed class InboxProcessorJob(
    IServiceScopeFactory scopeFactory, ITenantStore tenantStore,
    TimeProvider clock, ILogger<InboxProcessorJob> logger)
    : IJob
{
    private const int _batchSize = 20;

    public Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("tenantId")!;
        return TenantQuartzRunner.RunForTenantAsync(
            scopeFactory, tenantStore, logger, "Inbox processing (Quartz)", tenantId, ProcessTenantBatchAsync, context.CancellationToken);
    }

    private async Task ProcessTenantBatchAsync(IServiceProvider sp, TenantInfo tenant, CancellationToken ct)
    {
        var db = sp.GetRequiredService<AeroFlightDbContext>();
        var processors = sp.GetServices<IInboundMessageProcessor>().ToDictionary(p => p.MessageType);

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var claimedIds = await TenantQuartzRunner.ClaimPendingIdsAsync(db, PendingClaimTarget.InboxMessage, _batchSize, ct);
        if (claimedIds.Count == 0)
        {
            await tx.CommitAsync(ct);
            return;
        }

        var pending = await db.InboxMessageSet.Where(m => claimedIds.Contains(m.Id)).ToListAsync(ct);
        var processed = 0;

        foreach (var message in pending)
        {
            if (!processors.TryGetValue(message.MessageType, out var processor))
                continue;

            // A processor's own SaveChanges (e.g. an insert violating a FK) can abort the whole
            // Postgres transaction, not just that statement — every later command on it, including
            // the batch-level SaveChanges that would have persisted MarkFailed, would then fail too,
            // silently discarding the failure bookkeeping and leaving ProcessedAt null forever (the
            // message gets reclaimed and retried every run). The savepoint isolates one message's
            // failure so the transaction stays usable for the rest of the batch and for recording
            // this message as failed-but-processed.
            await tx.CreateSavepointAsync("inbox_message", ct);
            try
            {
                await processor.ProcessAsync(message.Payload, ct);
                message.MarkProcessed(clock.GetUtcNow());
                await db.SaveChangesAsync(ct);
                await tx.ReleaseSavepointAsync("inbox_message", ct);
                processed++;
            }
            catch (Exception ex)
            {
                await tx.RollbackToSavepointAsync("inbox_message", ct);

                // RollbackToSavepointAsync only undoes the DATABASE side — the change tracker still
                // holds whatever the failed processor call added/modified (e.g. the new flight that
                // violated an FK). Left tracked, the very next SaveChanges (persisting MarkFailed
                // below) would try to re-insert that same poisoned entity and throw the same error
                // again, uncaught this time, aborting the whole batch exactly like before. Detach
                // everything the failed call touched except the InboxMessage row itself.
                foreach (var entry in db.ChangeTracker.Entries().Where(e => e.Entity != message && e.State != EntityState.Unchanged).ToList())
                    entry.State = EntityState.Detached;

                message.MarkFailed(ex.Message, clock.GetUtcNow());
                await db.SaveChangesAsync(ct);
                logger.LogError(ex, "Failed to process inbox message {Id} for tenant {TenantId}.", message.Id, tenant.Id);
            }
        }

        await tx.CommitAsync(ct);

        logger.LogInformation("Inbox processing handled {Processed}/{Claimed} messages for tenant {TenantId}.",
            processed, claimedIds.Count, tenant.Id);
    }
}
