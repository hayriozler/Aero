using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Common;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace AeroFlight.Application.Metadatas;

public sealed class MetadataAuditQueryHandler<TEntity, TLog>(IAeroFlightDbContext db)
    : IQueryHandler<MetadataAuditQuery<TEntity>, IReadOnlyList<AuditEntryResponse>>
    where TLog : AuditLogBase
{
    public async Task<IReadOnlyList<AuditEntryResponse>> HandleAsync(MetadataAuditQuery<TEntity> query, CancellationToken ct)
    {
        // Metadata entities key on a single string Id, so this matches exactly what AuditInterceptor
        // wrote as PrimaryKey ({"Id":"<code>"}, default JsonSerializer settings, same as here).
        var primaryKeyJson = JsonSerializer.Serialize(new Dictionary<string, object?> { ["Id"] = query.Id });

        var logs = await db.Query<TLog>()
            .Where(l => l.PrimaryKey == primaryKeyJson)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(ct);

        return [.. logs.Select(l => new AuditEntryResponse(l.Id, l.Action, l.ChangedBy, l.Timestamp, l.Snapshot))];
    }
}
