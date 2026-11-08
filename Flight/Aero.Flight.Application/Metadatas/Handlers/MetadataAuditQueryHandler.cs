using Aero.Flights.Infrastructure.Persistence;
using Aero.Shared.Application;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Domain;
using System.Text.Json;

namespace Aero.Flights.Application.Metadatas.Handlers;

public sealed class MetadataAuditQueryHandler<TEntity, TLog>(AeroFlightDbContext db)
    : IQueryHandler<MetadataAuditQuery<TEntity>, IReadOnlyList<AuditEntryResponse>>
    where TLog : AuditLogBase
{
    public async Task<IReadOnlyList<AuditEntryResponse>> HandleAsync(MetadataAuditQuery<TEntity> query, CancellationToken ct)
    {
        var primaryKeyJson = JsonSerializer.Serialize(new Dictionary<string, object?> { ["Id"] = query.Id });

        var logs = await db.Query<TLog>()
            .Where(l => l.PrimaryKey == primaryKeyJson)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(ct);

        return [.. logs.Select(l => new AuditEntryResponse(l.Id, l.Action, l.ChangedBy, l.Timestamp, l.Snapshot))];
    }
}
