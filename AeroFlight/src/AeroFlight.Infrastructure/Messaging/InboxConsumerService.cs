using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Tenancy;
using AeroFlight.Contracts.Topology;
using AeroFlight.Domain.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AeroFlight.Infrastructure.Messaging;

internal sealed class InboxConsumerService(
    IServiceScopeFactory scopeFactory, IMessageBus messageBus, ITenantStore tenantStore,
    ILogger<InboxConsumerService> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        messageBus.SubscribeAsync(QueueNames.AeroFlightInboundMessages, ExchangeNames.Inbound, "#",
            (delivery, ct) => PersistAsync(delivery, ct), stoppingToken);

    private async Task<bool> PersistAsync(MessageBusDelivery delivery, CancellationToken ct)
    {
        var tenantId = ReadTenantHeader(delivery.Headers);
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            logger.LogWarning("Dropping inbound message with no 'tenant' header; routing key {RoutingKey}.", delivery.RoutingKey);
            return false;
        }

        var tenant = await tenantStore.FindAsync(tenantId, ct);
        if (tenant is null || !tenant.IsActive)
        {
            logger.LogWarning("Dropping inbound message for unknown tenant {TenantId}.", tenantId);
            return false;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ITenantContext>().Set(tenant);
        var db = scope.ServiceProvider.GetRequiredService<IAeroFlightDbContext>();
        var clock = scope.ServiceProvider.GetRequiredService<TimeProvider>();

        var payload = Encoding.UTF8.GetString(delivery.Body.Span);
        db.Add(InboxMessage.Create(delivery.RoutingKey, delivery.RoutingKey, payload, clock.GetUtcNow()));
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static string? ReadTenantHeader(IReadOnlyDictionary<string, object?>? headers)
    {
        if (headers is not null && headers.TryGetValue(MessageHeaders.Tenant, out var raw))
        {
            return raw switch
            {
                byte[] bytes => Encoding.UTF8.GetString(bytes),
                string text => text,
                _ => raw?.ToString(),
            };
        }

        return null;
    }
}
