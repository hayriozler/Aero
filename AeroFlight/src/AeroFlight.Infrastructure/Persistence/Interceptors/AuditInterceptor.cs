using AeroFlight.Domain.Common;
using AeroFlight.Domain.Ops.Resources;
using AeroFlight.Infrastructure.Persistence.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;

namespace AeroFlight.Infrastructure.Persistence.Interceptors;

public sealed class AuditInterceptor(IHttpContextAccessor httpContextAccessor, TimeProvider timeProvider)
    : SaveChangesInterceptor
{
    private bool _writingAudit;
    private readonly List<PendingAudit> _pending = [];

    private static readonly Lazy<Dictionary<Type, Func<AuditLogBase>>> _auditFactoriesLazy =
        new(BuildFactories, LazyThreadSafetyMode.ExecutionAndPublication);

    private static Dictionary<Type, Func<AuditLogBase>> BuildFactories() =>
        AuditLogTypeRegistry.All
            .ToDictionary(
                kv => kv.Key,
                kv => Expression.Lambda<Func<AuditLogBase>>(Expression.New(kv.Value)).Compile());

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            CaptureChanges(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            CaptureChanges(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    private void CaptureChanges(DbContext context)
    {
        if (_writingAudit)
            return;

        _pending.Clear();
        var now = timeProvider.GetUtcNow();
        var username = ResolveUser();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
                ApplyCreate(entry.Entity, username, now);
            if (entry.State is EntityState.Modified or EntityState.Deleted)
                ApplyUpdate(entry.Entity, username, now);

            if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
                && _auditFactoriesLazy.Value.ContainsKey(entry.Entity.GetType()))
            {
                _pending.Add(new PendingAudit(entry));
            }
        }
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (_writingAudit || _pending.Count == 0 || eventData.Context is null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var username = ResolveUser();
        var auditLogs = BuildAuditLogs(_pending, username, eventData.Context, timeProvider.GetUtcNow().UtcDateTime);

        _writingAudit = true;
        try
        {
            foreach (var log in auditLogs)
                eventData.Context.Add(log);

            // CancellationToken.None: audit must persist even if the HTTP request is aborted
            await eventData.Context.SaveChangesAsync(CancellationToken.None);
        }
        finally
        {
            _writingAudit = false;
            _pending.Clear();
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static List<AuditLogBase> BuildAuditLogs(
        List<PendingAudit> pending, string changedBy, DbContext context, DateTime now)
    {
        var logs = new List<AuditLogBase>(pending.Count * 2);

        foreach (var p in pending)
        {
            if (_auditFactoriesLazy.Value.TryGetValue(p.EntityType, out var factory))
            {
                var log = factory();
                log.Action = p.State.ToString();
                log.Timestamp = now;
                log.ChangedBy = changedBy;

                var keyValues = p.State == EntityState.Added
                    ? ResolveCurrentKey(context, p.EntityRef)
                    : p.KeyValues;

                log.PrimaryKey = JsonSerializer.Serialize(keyValues);
                log.Snapshot = BuildSnapshot(p, context);
                if (p.EntityRef is FlightResourceBase resource)
                    log.FlightId = resource.FlightId;
                else if (p.EntityRef is OpsFlightParkPosition parkPosition)
                    log.FlightId = parkPosition.ArrivalFlightId ?? parkPosition.DepartureFlightId;

                logs.Add(log);
            }

            if (CrossAuditRegistry.TryGet(p.EntityType, out var rule))
            {
                var crossAction = p.State switch
                {
                    EntityState.Added => rule.ActionPrefix + "Added",
                    EntityState.Modified => rule.ActionPrefix + "Modified",
                    EntityState.Deleted => rule.ActionPrefix + "Removed",
                    _ => null
                };
                if (crossAction is null) continue;

                var crossLog = (AuditLogBase)Activator.CreateInstance(rule.TargetLogType)!;
                crossLog.Action = crossAction;
                crossLog.Timestamp = now;
                crossLog.ChangedBy = changedBy;
                crossLog.PrimaryKey = rule.ExtractTargetKey(p.EntityRef);
                crossLog.Snapshot = BuildSnapshot(p, context);

                logs.Add(crossLog);
            }
        }

        return logs;
    }

    private static string BuildSnapshot(PendingAudit p, DbContext context) =>
        p.State switch
        {
            EntityState.Added => JsonSerializer.Serialize(ResolveCurrentValues(context, p.EntityRef)),
            EntityState.Deleted => JsonSerializer.Serialize(p.OriginalValues
                .ToDictionary(
                    kv => AuditDisplayNameRegistry.Resolve(p.EntityType, kv.Key),
                    kv => ForDisplay(kv.Value))),
            EntityState.Modified => BuildModifiedSnapshot(p),
            _ => "{}"
        };

    private static string BuildModifiedSnapshot(PendingAudit p)
    {
        var changes = new Dictionary<string, object?>();

        foreach (var prop in p.CurrentValues.Keys)
        {
            if (!p.OriginalValues.TryGetValue(prop, out var oldVal)) continue;

            var newVal = p.CurrentValues[prop];
            if (Equals(oldVal, newVal)) continue;

            var displayKey = AuditDisplayNameRegistry.Resolve(p.EntityType, prop);
            changes[displayKey] = new { old = ForDisplay(oldVal), @new = ForDisplay(newVal) };
        }

        return JsonSerializer.Serialize(changes);
    }

    // The change tracker hands back raw CLR values — enum-typed properties would otherwise
    // serialize as their underlying int (System.Text.Json's default). Fall back to the member
    // name instead of a bare int for any enum-typed property on an audited entity.
    private static object? ForDisplay(object? value) => value switch
    {
        Enum e => e.ToString(),
        _ => value,
    };

    private static Dictionary<string, object?> ResolveCurrentKey(DbContext context, object entity)
    {
        var entry = context.Entry(entity);
        return entry.Metadata.FindPrimaryKey()?.Properties
            .ToDictionary(p => p.Name, p => (object?)entry.CurrentValues[p])
            ?? [];
    }

    private static Dictionary<string, object?> ResolveCurrentValues(DbContext context, object entity)
    {
        var entry = context.Entry(entity);
        var entityType = entity.GetType();
        return entry.CurrentValues.Properties
            .Where(p => !AuditDisplayNameRegistry.IsIgnored(entityType, p.Name))
            .ToDictionary(
                p => AuditDisplayNameRegistry.Resolve(entityType, p.Name),
                p => ForDisplay(entry.CurrentValues[p]));
    }

    private static void ApplyCreate(object entity, string username, DateTimeOffset now)
    {
        if (entity is AuditBase<long> l) l.SetAuditOnCreate(username, now);
        else if (entity is AuditBase<int> i) i.SetAuditOnCreate(username, now);
        else if (entity is AuditBase<string> s) s.SetAuditOnCreate(username, now);
    }

    private static void ApplyUpdate(object entity, string username, DateTimeOffset now)
    {
        if (entity is AuditBase<long> l) l.SetAuditOnUpdate(username, now);
        else if (entity is AuditBase<int> i) i.SetAuditOnUpdate(username, now);
        else if (entity is AuditBase<string> s) s.SetAuditOnUpdate(username, now);
    }

    private string ResolveUser() =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
        ?? httpContextAccessor.HttpContext?.User.FindFirstValue("unique_name")
        ?? "system";

    private sealed class PendingAudit
    {
        private static readonly HashSet<string> _auditBaseProps =
            ["Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy"];

        public Type EntityType { get; }
        public EntityState State { get; }
        public object EntityRef { get; }
        public Dictionary<string, object?> KeyValues { get; }
        public Dictionary<string, object?> OriginalValues { get; }
        public Dictionary<string, object?> CurrentValues { get; }

        public PendingAudit(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            EntityType = entry.Entity.GetType();
            State = entry.State;
            EntityRef = entry.Entity;

            KeyValues = entry.Metadata.FindPrimaryKey()?.Properties
                .ToDictionary(p => p.Name, p => (object?)entry.CurrentValues[p])
                ?? [];

            OriginalValues = entry.OriginalValues.Properties
                .Where(p => !_auditBaseProps.Contains(p.Name)
                         && !AuditDisplayNameRegistry.IsIgnored(EntityType, p.Name))
                .ToDictionary(p => p.Name, p => entry.OriginalValues[p]);

            CurrentValues = entry.State == EntityState.Deleted
                ? OriginalValues
                : entry.CurrentValues.Properties
                    .Where(p => !_auditBaseProps.Contains(p.Name)
                             && !AuditDisplayNameRegistry.IsIgnored(EntityType, p.Name))
                    .ToDictionary(p => p.Name, p => entry.CurrentValues[p]);
        }
    }
}
