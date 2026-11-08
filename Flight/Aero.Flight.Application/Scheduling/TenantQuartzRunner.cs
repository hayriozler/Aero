using Aero.Shared.Application.Tenancy;
using Aero.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aero.Flights.Application.Scheduling;

internal static class TenantQuartzRunner
{
    public static async Task RunForTenantAsync(
        IServiceScopeFactory scopeFactory, ITenantStore tenantStore, ILogger logger, string operation,
        string tenantId, Func<IServiceProvider, TenantInfo, CancellationToken, Task> action, CancellationToken ct)
    {
        var tenant = await tenantStore.FindAsync(tenantId, ct);
        if (tenant is null || !tenant.IsActive)
        {
            logger.LogWarning("{Operation} skipped: tenant {TenantId} not found or inactive.", operation, tenantId);
            return;
        }

        try
        {
            logger.LogInformation("{Operation} running for tenant {TenantId}.", operation, tenant.Id);
            await using var scope = scopeFactory.CreateAsyncScope();
            scope.ServiceProvider.GetRequiredService<ITenantContext>().Set(tenant);
            await action(scope.ServiceProvider, tenant, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "{Operation} failed for tenant {TenantId}.", operation, tenant.Id);
        }
    }

    public static Task<List<long>> ClaimPendingIdsAsync(
        DbContext db, PendingClaimTarget target, int batchSize, CancellationToken ct)
    {
        var (table, pendingColumn) = target switch
        {
            PendingClaimTarget.OutboxMessage => ("OutboxMessage", "PublishedAt"),
            PendingClaimTarget.InboxMessage => ("InboxMessage", "ProcessedAt"),
            _ => throw new ArgumentOutOfRangeException(nameof(target)),
        };

        var sql = $"SELECT \"Id\" FROM ops.\"{table}\" WHERE \"{pendingColumn}\" IS NULL "
                + $"ORDER BY \"Id\" LIMIT {batchSize} FOR UPDATE SKIP LOCKED";
        return db.Database.SqlQueryRaw<long>(sql).ToListAsync(ct);
    }
}

internal enum PendingClaimTarget
{
    OutboxMessage,
    InboxMessage,
}
