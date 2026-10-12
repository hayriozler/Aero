using AeroFlight.Application.Metadatas;
using AeroFlight.Contracts.Outbound;
using AeroFlight.Contracts.Topology;
using AeroFlight.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace AeroFlight.Infrastructure.Persistence.Interceptors;

// Registered after AuditInterceptor so audit fields are populated in the emitted payload.
// Writes the outbox row in the same SaveChanges as the change (atomic, single transaction).
public sealed class MetadataOutboxInterceptor(IEnumerable<IMetadataOutboxMapper> mappers, TimeProvider clock)
    : SaveChangesInterceptor
{
    private readonly Dictionary<Type, IMetadataOutboxMapper> _mappers = mappers.ToDictionary(m => m.EntityType);

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Emit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        Emit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void Emit(DbContext? context)
    {
        if (context is null) return;

        var now = clock.GetUtcNow();
        var messages = new List<OutboxMessage>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            var operation = entry.State switch
            {
                EntityState.Added => RoutingKeys.MetadataData.Created,
                EntityState.Modified => RoutingKeys.MetadataData.Updated,
                EntityState.Deleted => RoutingKeys.MetadataData.Deleted,
                _ => null,
            };
            if (operation is null || !_mappers.TryGetValue(entry.Entity.GetType(), out var mapper))
                continue;

            var code = mapper.GetCode(entry.Entity);
            JsonElement? data = operation == RoutingKeys.MetadataData.Deleted
                ? null
                : JsonSerializer.SerializeToElement(mapper.ToResponse(entry.Entity));

            var message = new MetadataDataChanged(mapper.EntityName, operation, code, data, now);
            messages.Add(OutboxMessage.Create(
                ExchangeNames.Metadata,
                RoutingKeys.MetadataData.For(mapper.EntityName, operation),
                nameof(MetadataDataChanged),
                JsonSerializer.Serialize(message),
                now));
        }

        foreach (var message in messages)
            context.Add(message);
    }
}
