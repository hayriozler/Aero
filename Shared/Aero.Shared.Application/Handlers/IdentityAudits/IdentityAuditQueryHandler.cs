using Aero.Flights.Application.Common;
using Aero.Shared.Domain;
using Aero.Shared.Infrastructure.Persistence;
using System.Text.Json;

namespace Aero.Shared.Application.Handlers.IdentityAudits;

public sealed class IdentityAuditQueryHandler<TEntity, TLog>(IDataStoreContext db)
    : IQueryHandler<MetadataAuditQuery<TEntity>, IReadOnlyList<AuditEntryResponse>>
    where TLog : AuditLogBase
{
    public async Task<IReadOnlyList<AuditEntryResponse>> HandleAsync(MetadataAuditQuery<TEntity> query, CancellationToken ct)
    {
        var primaryKeyJson = JsonSerializer.Serialize(new Dictionary<string, object?> { ["Id"] = int.Parse(query.Id) });

        var logs = await db.Query<TLog>()
            .Where(l => l.PrimaryKey == primaryKeyJson)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(ct);

        return logs.Select(l => new AuditEntryResponse(l.Id, l.Action, l.ChangedBy, l.Timestamp, l.Snapshot)).ToList();
    }
}

