using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AeroFlight.Contracts.Outbound;
using AeroFlight.Contracts.Topology;
using AeroFlight.Domain.Common;
using AeroFlight.Domain.Configuration;
using AeroFlight.Domain.Messaging;
using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Ops.Passengers;
using AeroFlight.Domain.Ops.Resources;
using AeroFlight.Domain.Ops.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AeroFlight.Infrastructure.Persistence.Interceptors;

public sealed class FlightOutboxInterceptor(IHttpContextAccessor httpContextAccessor, TimeProvider clock)
    : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(ExcludeFlightBackMetadatas),
    };
    private static readonly HashSet<string> _excludedFromChanges = ["Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy"];

    private static void ExcludeFlightBackMetadatas(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type != typeof(OpsFlightParkPosition))
            return;

        for (var i = typeInfo.Properties.Count - 1; i >= 0; i--)
        {
            if (typeInfo.Properties[i].Name is "arrivalFlight" or "departureFlight")
                typeInfo.Properties.RemoveAt(i);
        }
    }    

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        if (eventData.Context is not null)
            await EmitAsync(eventData.Context, allowDbFallback: true, ct);

        return await base.SavingChangesAsync(eventData, result, ct);
    }

    private async Task EmitAsync(DbContext context, bool allowDbFallback, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var groups = new Dictionary<(Type FlightType, long FlightId), FlightEventGroup>();

        FlightEventGroup GroupFor(Type flightType, long flightId)
        {
            var key = (flightType, flightId);
            if (!groups.TryGetValue(key, out var group))
                groups[key] = group = new FlightEventGroup();
            return group;
        }

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is ArrivalFlight or DepartureFlight && entry.State is EntityState.Added or EntityState.Modified)
            {
                var flight = (Flight)entry.Entity;
                var group = GroupFor(flight.GetType(), flight.Id);
                group.IsNew = entry.State == EntityState.Added;
                group.Changes = entry.State == EntityState.Modified ? BuildChanges(entry) : [];
                group.Snapshot = BuildSnapshot(entry, flight);
            }
            else if (entry.Entity is ResourceBase resource && entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                var (flightType, kind) = ResolveResourceKind(resource);
                if (flightType is null)
                    continue;

                GroupFor(flightType, resource.FlightId).Track(kind, entry.State, resource);
            }
            else if (entry.Entity is OpsFlightParkPosition parkPosition && entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                if (parkPosition.ArrivalFlightId is { } arrivalFlightId)
                    GroupFor(typeof(ArrivalFlight), arrivalFlightId).TrackParkPosition(entry.State, parkPosition);
                if (parkPosition.DepartureFlightId is { } departureFlightId)
                    GroupFor(typeof(DepartureFlight), departureFlightId).TrackParkPosition(entry.State, parkPosition);
            }
            else if (entry.Entity is OpsRouting routing && entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                var flightType = routing is OpsArrivalRouting ? typeof(ArrivalFlight) : typeof(DepartureFlight);
                GroupFor(flightType, routing.FlightId).Track(ResourceKind.Routings, entry.State, routing);
            }
            else if (entry.Entity is OpsPassenger passenger && entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                var flightType = passenger is OpsArrivalPassenger ? typeof(ArrivalFlight) : typeof(DepartureFlight);
                GroupFor(flightType, passenger.FlightId).Track(ResourceKind.Passengers, entry.State, passenger);
            }
        }

        if (groups.Count == 0)
            return;

        var changedBy = ResolveUser();
        var tenantTimeZone = await ResolveTenantTimeZoneAsync(context, ct);

        foreach (var ((flightType, flightId), group) in groups)
        {
            if (group.Snapshot is null && allowDbFallback)
                group.Snapshot = await LoadSnapshotAsync(context, flightType, flightId, ct);
            if (group.Snapshot is null)
                continue;

            var isArrival = flightType == typeof(ArrivalFlight);
            var routingKey = (isArrival, group.IsNew) switch
            {
                (true, true) => RoutingKeys.Outbound.FlightArrivalCreated,
                (true, false) => RoutingKeys.Outbound.FlightArrivalUpdated,
                (false, true) => RoutingKeys.Outbound.FlightDepartureCreated,
                (false, false) => RoutingKeys.Outbound.FlightDepartureUpdated,
            };

            if (allowDbFallback)
                await PopulateCurrentResourcesAsync(context, flightType, flightId, isArrival, group, ct);

            var flightJson = JsonSerializer.SerializeToElement(group.Snapshot, _jsonOptions);
            var changes = group.Changes ?? [];

            FlightChanged payload = isArrival
                ? new ArrivalFlightChanged(Guid.NewGuid(), routingKey, now, changedBy, tenantTimeZone, flightJson, changes,
                    group.GatesResources, group.BeltsResources, group.ParkPositionsResources, group.RoutingsResources, group.PassengersResources)
                : new DepartureFlightChanged(Guid.NewGuid(), routingKey, now, changedBy, tenantTimeZone, flightJson, changes,
                    group.GatesResources, group.BeltsResources, group.ParkPositionsResources, group.RoutingsResources, group.CountersResources, group.PassengersResources);

            context.Add(OutboxMessage.Create(ExchangeNames.Flights, routingKey, payload.GetType().Name,
                JsonSerializer.Serialize(payload, payload.GetType(), _jsonOptions), now));
        }
    }

    private static async Task PopulateCurrentResourcesAsync(
        DbContext context, Type flightType, long flightId, bool isArrival, FlightEventGroup group, CancellationToken ct)
    {
        var snapshot = group.Snapshot!;

        if (isArrival)
        {
            var gates = await BuildCurrentEntitiesAsync<OpsArrivalGate>(context, flightId, group.Gates, ct);
            var belts = await BuildCurrentEntitiesAsync<OpsArrivalBelt>(context, flightId, group.Belts, ct);
            group.GatesResources = ToResourceChanged(gates);
            group.BeltsResources = ToResourceChanged(belts);
            group.RoutingsResources = await BuildCurrentAsync<OpsArrivalRouting>(context, flightId, group.Routings, ct);
            group.PassengersResources = await BuildCurrentAsync<OpsArrivalPassenger>(context, flightId, group.Passengers, ct);

            ApplyRollup(snapshot, "gates", "gateStatusCode", "deboardingBegin", "deboardingEnd", gates.Select(g => g.Entity));
            ApplyRollup(snapshot, "belts", "beltStatusCode", "beltBegin", "beltEnd", belts.Select(b => b.Entity));
        }
        else
        {
            var gates = await BuildCurrentEntitiesAsync<OpsDepartureGate>(context, flightId, group.Gates, ct);
            var belts = await BuildCurrentEntitiesAsync<OpsDepartureBelt>(context, flightId, group.Belts, ct);
            var counters = await BuildCurrentEntitiesAsync<OpsCounter>(context, flightId, group.Counters, ct);
            group.GatesResources = ToResourceChanged(gates);
            group.BeltsResources = ToResourceChanged(belts);
            group.CountersResources = ToResourceChanged(counters);
            group.RoutingsResources = await BuildCurrentAsync<OpsDepartureRouting>(context, flightId, group.Routings, ct);
            group.PassengersResources = await BuildCurrentAsync<OpsDeparturePassenger>(context, flightId, group.Passengers, ct);

            ApplyRollup(snapshot, "gates", "gateStatusCode", "gateBegin", "gateEnd", gates.Select(g => g.Entity));
            ApplyRollup(snapshot, "belts", "beltStatusCode", "beltBegin", "beltEnd", belts.Select(b => b.Entity));
            ApplyRollup(snapshot, "counters", "counterStatusCode", "counterBegin", "counterEnd", counters.Select(c => c.Entity));
        }

        group.ParkPositionsResources = await BuildCurrentParkPositionsAsync(context, flightType, flightId, group.ParkPositions, ct);
    }

    private static void ApplyRollup(
        Dictionary<string, object?> snapshot, string codesKey, string statusKey, string beginKey, string endKey,
        IEnumerable<ResourceBase> resources)
    {
        var list = resources.ToList();
        snapshot[codesKey] = ResourceRollup.Codes(list);
        snapshot[statusKey] = ResourceRollup.StatusCode(list);
        snapshot[beginKey] = ResourceRollup.Begin(list);
        snapshot[endKey] = ResourceRollup.End(list);
    }

    private static async Task<List<ResourceChanged>> BuildCurrentAsync<TResource>(
        DbContext context, long flightId, List<(EntityState State, FlightResourceBase Entity)> tracked, CancellationToken ct)
        where TResource : FlightResourceBase
    {
        var merged = await BuildCurrentEntitiesAsync<TResource>(context, flightId, tracked, ct);
        return ToResourceChanged(merged);
    }

    private static async Task<List<(string Operation, TResource Entity)>> BuildCurrentEntitiesAsync<TResource>(
        DbContext context, long flightId, List<(EntityState State, FlightResourceBase Entity)> tracked, CancellationToken ct)
        where TResource : FlightResourceBase
    {
        var dbRows = await context.Set<TResource>().AsNoTracking().Where(r => r.FlightId == flightId).ToListAsync(ct);
        var trackedTyped = tracked.Select(t => (t.State, Entity: (TResource)t.Entity)).ToList();
        return MergeEntities(dbRows, trackedTyped, r => r.Id);
    }

    private static async Task<List<ResourceChanged>> BuildCurrentParkPositionsAsync(
        DbContext context, Type flightType, long flightId,
        List<(EntityState State, OpsFlightParkPosition Entity)> tracked, CancellationToken ct)
    {
        var dbRows = flightType == typeof(ArrivalFlight)
            ? await context.Set<OpsFlightParkPosition>().AsNoTracking().Where(p => p.ArrivalFlightId == flightId).ToListAsync(ct)
            : await context.Set<OpsFlightParkPosition>().AsNoTracking().Where(p => p.DepartureFlightId == flightId).ToListAsync(ct);

        return ToResourceChanged(MergeEntities(dbRows, tracked, r => r.Id));
    }

    // Combines the already-persisted (pre-transaction) rows with whatever this transaction touched:
    // Modified rows replace their DB counterpart, Deleted rows drop out entirely (they no longer
    // exist), Added rows (no real id assigned yet at this point) are always included. Each surviving
    // row is tagged with what happened to it this transaction, or "unchanged" if it wasn't touched.
    private static List<(string Operation, TEntity Entity)> MergeEntities<TEntity>(
        List<TEntity> dbRows, List<(EntityState State, TEntity Entity)> tracked, Func<TEntity, long> idOf)
        where TEntity : notnull
    {
        var byId = dbRows.ToDictionary(idOf);
        var added = new List<TEntity>();
        var stateById = new Dictionary<long, EntityState>();

        foreach (var (state, entity) in tracked)
        {
            if (state == EntityState.Added)
            {
                added.Add(entity);
                continue;
            }

            var id = idOf(entity);
            stateById[id] = state;
            if (state == EntityState.Deleted)
                byId.Remove(id);
            else
                byId[id] = entity;
        }

        var result = new List<(string, TEntity)>();
        foreach (var r in byId.Values)
        {
            var id = idOf(r);
            var operation = stateById.TryGetValue(id, out var s) ? OperationOf(s) : "unchanged";
            result.Add((operation, r));
        }
        foreach (var r in added)
            result.Add(("inserted", r));

        return result;
    }

    private static List<ResourceChanged> ToResourceChanged<TEntity>(List<(string Operation, TEntity Entity)> merged)
        where TEntity : notnull =>
        [.. merged.Select(m => new ResourceChanged(m.Operation, JsonSerializer.SerializeToElement(m.Entity, m.Entity!.GetType(), _jsonOptions)))];

    private static string OperationOf(EntityState state) => state switch
    {
        EntityState.Added => "inserted",
        EntityState.Modified => "updated",
        _ => "deleted",
    };

    private static (Type? FlightType, ResourceKind Kind) ResolveResourceKind(ResourceBase resource) => resource switch
    {
        OpsArrivalGate => (typeof(ArrivalFlight), ResourceKind.Gates),
        OpsArrivalBelt => (typeof(ArrivalFlight), ResourceKind.Belts),
        OpsDepartureGate => (typeof(DepartureFlight), ResourceKind.Gates),
        OpsDepartureBelt => (typeof(DepartureFlight), ResourceKind.Belts),
        OpsCounter => (typeof(DepartureFlight), ResourceKind.Counters),
        _ => (null, default),
    };

    private static Dictionary<string, ChangedField> BuildChanges(EntityEntry entry)
    {
        var changes = new Dictionary<string, ChangedField>();

        foreach (var prop in entry.OriginalValues.Properties)
        {
            if (_excludedFromChanges.Contains(prop.Name)) continue;

            var oldValue = entry.OriginalValues[prop];
            var newValue = entry.CurrentValues[prop];
            if (Equals(oldValue, newValue)) continue;

            changes[JsonNamingPolicy.CamelCase.ConvertName(prop.Name)] = new ChangedField(oldValue, newValue);
        }

        return changes;
    }

    private static Dictionary<string, object?> BuildSnapshot(EntityEntry entry, Flight flight)
    {
        var snapshot = SnapshotOf(entry);

        if (flight is ArrivalFlight arrival)
        {
            snapshot["originAirportCode"] = arrival.OriginAirportCode;
            snapshot["routingSummary"] = arrival.RoutingSummary;
            snapshot["gates"] = arrival.Gates;
            snapshot["gateStatusCode"] = arrival.GateStatusCode;
            snapshot["deboardingBegin"] = arrival.DeboardingBegin;
            snapshot["deboardingEnd"] = arrival.DeboardingEnd;
            snapshot["belts"] = arrival.Belts;
            snapshot["beltStatusCode"] = arrival.BeltStatusCode;
            snapshot["beltBegin"] = arrival.BeltBegin;
            snapshot["beltEnd"] = arrival.BeltEnd;
        }
        else if (flight is DepartureFlight departure)
        {
            snapshot["destinationAirportCode"] = departure.DestinationAirportCode;
            snapshot["routingSummary"] = departure.RoutingSummary;
            snapshot["gates"] = departure.Gates;
            snapshot["gateStatusCode"] = departure.GateStatusCode;
            snapshot["gateBegin"] = departure.GateBegin;
            snapshot["gateEnd"] = departure.GateEnd;
            snapshot["belts"] = departure.Belts;
            snapshot["beltStatusCode"] = departure.BeltStatusCode;
            snapshot["beltBegin"] = departure.BeltBegin;
            snapshot["beltEnd"] = departure.BeltEnd;
            snapshot["counters"] = departure.Counters;
            snapshot["counterStatusCode"] = departure.CounterStatusCode;
            snapshot["counterBegin"] = departure.CounterBegin;
            snapshot["counterEnd"] = departure.CounterEnd;
        }

        return snapshot;
    }

    private static Dictionary<string, object?> SnapshotOf(EntityEntry entry)
    {
        var values = entry.State == EntityState.Deleted ? entry.OriginalValues : entry.CurrentValues;
        return values.Properties.ToDictionary(p => JsonNamingPolicy.CamelCase.ConvertName(p.Name), p => values[p]);
    }

    private static async Task<Dictionary<string, object?>?> LoadSnapshotAsync(DbContext context, Type flightType, long flightId, CancellationToken ct)
    {
        if (flightType == typeof(ArrivalFlight))
        {
            var f = await context.Set<ArrivalFlight>().AsNoTracking().Include(x => x.Routings)
                .FirstOrDefaultAsync(x => x.Id == flightId, ct);
            return f is null ? null : BuildSnapshot(context.Entry(f), f);
        }
        else
        {
            var f = await context.Set<DepartureFlight>().AsNoTracking().Include(x => x.Routings)
                .FirstOrDefaultAsync(x => x.Id == flightId, ct);
            return f is null ? null : BuildSnapshot(context.Entry(f), f);
        }
    }

    private string ResolveUser() =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
        ?? httpContextAccessor.HttpContext?.User.FindFirstValue("unique_name")
        ?? "system";

    // Read straight off the DbContext already in hand rather than IAeroConfigService — that
    // service depends on IAeroFlightDbContext, and this interceptor is constructed while AeroFlightDbContext
    // itself is still being built (inside its own options factory), so injecting it here would
    // be a circular dependency.
    private static async Task<string> ResolveTenantTimeZoneAsync(DbContext context, CancellationToken ct) =>
        await context.Set<AeroConfiguration>()
            .Where(c => c.ConfigurationName == "Display.TimeZone")
            .Select(c => c.Value)
            .FirstOrDefaultAsync(ct) ?? "UTC";

    private enum ResourceKind { Gates, Belts, Counters, Routings, Passengers }

    private sealed class FlightEventGroup
    {
        public bool IsNew;
        public Dictionary<string, object?>? Snapshot;
        public Dictionary<string, ChangedField>? Changes;
        public IReadOnlyList<ResourceChanged>? GatesResources;
        public IReadOnlyList<ResourceChanged>? BeltsResources;
        public IReadOnlyList<ResourceChanged>? CountersResources;
        public IReadOnlyList<ResourceChanged>? ParkPositionsResources;
        public IReadOnlyList<ResourceChanged>? RoutingsResources;
        public IReadOnlyList<ResourceChanged>? PassengersResources;

        public List<(EntityState State, FlightResourceBase Entity)> Gates { get; } = [];
        public List<(EntityState State, FlightResourceBase Entity)> Belts { get; } = [];
        public List<(EntityState State, FlightResourceBase Entity)> Counters { get; } = [];
        public List<(EntityState State, FlightResourceBase Entity)> Routings { get; } = [];
        public List<(EntityState State, FlightResourceBase Entity)> Passengers { get; } = [];
        public List<(EntityState State, OpsFlightParkPosition Entity)> ParkPositions { get; } = [];

        public void Track(ResourceKind kind, EntityState state, FlightResourceBase entity)
        {
            var list = kind switch
            {
                ResourceKind.Gates => Gates,
                ResourceKind.Belts => Belts,
                ResourceKind.Counters => Counters,
                ResourceKind.Passengers => Passengers,
                _ => Routings,
            };
            list.Add((state, entity));
        }

        public void TrackParkPosition(EntityState state, OpsFlightParkPosition entity) => ParkPositions.Add((state, entity));
    }
}
