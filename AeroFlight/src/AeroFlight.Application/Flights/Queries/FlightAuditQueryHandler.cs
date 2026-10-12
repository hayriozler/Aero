using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Metadatas;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AeroFlight.Application.Flights.Queries;

public sealed class FlightAuditQueryHandler<TEntity, TLog>(IAeroFlightDbContext db)
    : IQueryHandler<MetadataAuditQuery<TEntity>, IReadOnlyList<AuditEntryResponse>>
    where TLog : AuditLogBase
{
    public async Task<IReadOnlyList<AuditEntryResponse>> HandleAsync(MetadataAuditQuery<TEntity> query, CancellationToken ct)
    {
        var primaryKeyJson = JsonSerializer.Serialize(new Dictionary<string, object?> { ["Id"] = long.Parse(query.Id) });

        var logs = await db.Query<TLog>()
            .Where(l => l.PrimaryKey == primaryKeyJson)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(ct);

        return [.. logs.Select(l => new AuditEntryResponse(l.Id, l.Action, l.ChangedBy, l.Timestamp, l.Snapshot))];
    }
}
