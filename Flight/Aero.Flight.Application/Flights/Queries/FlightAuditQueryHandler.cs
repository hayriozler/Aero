using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.Metadatas;
using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Aero.Flights.Application.Flights.Queries;

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
