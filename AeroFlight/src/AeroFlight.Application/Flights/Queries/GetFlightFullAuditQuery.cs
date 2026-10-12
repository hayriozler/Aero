using System.Text.Json;
using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Audit.Ops;
using AeroFlight.Domain.Audit.Ops.Resources;
using AeroFlight.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.Queries;

public sealed record GetFlightFullAuditQuery(long FlightId, bool IsArrival);

// A flight's Gate/Belt/Counter allocations are separate entities with their own audit log tables,
// keyed by the resource's own Id — not the flight's — so the flight's own audit query never sees
// them. This merges the flight's audit trail with its resources' audit trails (label-prefixed so
// the source is clear), sorted together. Resource audit rows carry their own FlightId (set by
// AuditInterceptor), so this works even for a resource that has since been deleted — no dependency
// on the resource still existing in its live table.
public sealed class GetFlightFullAuditQueryHandler(IAeroFlightDbContext db) : IQueryHandler<GetFlightFullAuditQuery, IReadOnlyList<AuditEntryResponse>>
{
    public async Task<IReadOnlyList<AuditEntryResponse>> HandleAsync(GetFlightFullAuditQuery query, CancellationToken ct)
    {
        var flightKey = JsonSerializer.Serialize(new Dictionary<string, object?> { ["Id"] = query.FlightId });
        var entries = new List<AuditEntryResponse>();

        if (query.IsArrival)
        {
            entries.AddRange(await LoadFlightAsync<ArrivalFlightAudit>(flightKey, ct));
            entries.AddRange(await LoadResourceAsync<OpsArrivalGateAudit>("Gate", query.FlightId, ct));
            entries.AddRange(await LoadResourceAsync<OpsArrivalBeltAudit>("Belt", query.FlightId, ct));
        }
        else
        {
            entries.AddRange(await LoadFlightAsync<DepartureFlightAudit>(flightKey, ct));
            entries.AddRange(await LoadResourceAsync<OpsDepartureGateAudit>("Gate", query.FlightId, ct));
            entries.AddRange(await LoadResourceAsync<OpsDepartureBeltAudit>("Belt", query.FlightId, ct));
            entries.AddRange(await LoadResourceAsync<OpsCounterAudit>("Counter", query.FlightId, ct));
        }

        entries.AddRange(await LoadResourceAsync<OpsFlightParkPositionAudit>("Park Position", query.FlightId, ct));

        return [.. entries.OrderByDescending(e => e.Timestamp)];
    }

    private async Task<List<AuditEntryResponse>> LoadFlightAsync<TLog>(string flightPrimaryKeyJson, CancellationToken ct)
        where TLog : AuditLogBase
    {
        var logs = await db.Query<TLog>().Where(l => l.PrimaryKey == flightPrimaryKeyJson).ToListAsync(ct);
        return [.. logs.Select(l => new AuditEntryResponse(l.Id, l.Action, l.ChangedBy, l.Timestamp, l.Snapshot))];
    }

    private async Task<List<AuditEntryResponse>> LoadResourceAsync<TLog>(string label, long flightId, CancellationToken ct)
        where TLog : AuditLogBase
    {
        var logs = await db.Query<TLog>().Where(l => l.FlightId == flightId).ToListAsync(ct);
        return [.. logs.Select(l => new AuditEntryResponse(l.Id, $"{label}: {l.Action}", l.ChangedBy, l.Timestamp, l.Snapshot))];
    }
}
