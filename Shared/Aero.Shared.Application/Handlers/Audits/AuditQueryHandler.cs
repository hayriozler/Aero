using Aero.Shared.Application.Queries.Audits;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Domain;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Aero.Shared.Application.Handlers.Audits;

public sealed class AuditQueryHandler<TEntity, TLog>(IDataStoreContext db)
    : IQueryHandler<AuditQuery<TEntity>, IReadOnlyList<AuditEntryResponse>>
    where TLog : AuditLogBase
{
    public async Task<IReadOnlyList<AuditEntryResponse>> HandleAsync(AuditQuery<TEntity> query, CancellationToken ct)
    {
        var primaryKeyJson = JsonSerializer.Serialize(new Dictionary<string, object?> { ["Id"] = int.Parse(query.Id) });

        var logs = await db.Query<TLog>()
            .Where(l => l.PrimaryKey == primaryKeyJson)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(ct);

        return logs.Select(l => new AuditEntryResponse(l.Id, l.Action, l.CreatedBy, l.Timestamp, l.Snapshot)).ToList();
    }
}

