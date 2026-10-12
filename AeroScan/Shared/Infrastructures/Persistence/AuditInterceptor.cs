using Domain.Audits;
using Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;

namespace Infrastructure.Persistence;

public sealed class AuditInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
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
        if (_writingAudit || eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        _pending.Clear();

        var now = DateTime.UtcNow;
        var changedBy = ResolveUser();

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            var type = entry.Entity.GetType();

            if (IsAuditableEntity(type))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.CurrentValues[nameof(AuditableEntity<int>.CreatedAt)] = now;
                    entry.CurrentValues[nameof(AuditableEntity<int>.CreatedBy)] = changedBy;
                    entry.CurrentValues[nameof(AuditableEntity<int>.UpdatedAt)] = now;
                    entry.CurrentValues[nameof(AuditableEntity<int>.UpdatedBy)] = changedBy;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.CurrentValues[nameof(AuditableEntity<int>.UpdatedAt)] = now;
                    entry.CurrentValues[nameof(AuditableEntity<int>.UpdatedBy)] = changedBy;
                }
            }

            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted)) continue;
            if (!_auditFactoriesLazy.Value.ContainsKey(type)) continue;

            _pending.Add(new PendingAudit(entry));
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (_writingAudit || _pending.Count == 0 || eventData.Context is null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var changedBy = ResolveUser();

        var auditLogs = BuildAuditLogs(_pending, changedBy, eventData.Context);

        _writingAudit = true;
        try
        {
            foreach (var log in auditLogs)
                eventData.Context.Add(log);

            await eventData.Context.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _writingAudit = false;
            _pending.Clear();
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static List<AuditLogBase> BuildAuditLogs(List<PendingAudit> pending, string changedBy, DbContext context)
    {
        var now = DateTime.UtcNow;
        var logs = new List<AuditLogBase>(pending.Count * 2);

        foreach (var p in pending)
        {
            // ── Primary entity audit ──────────────────────────────────────
            if (_auditFactoriesLazy.Value.TryGetValue(p.EntityType, out var factory))
            {
                var log = factory();
                log.Action    = p.State.ToString();
                log.Timestamp = now;
                log.CreatedBy = changedBy;

                var keyValues = p.State == EntityState.Added
                    ? ResolveCurrentKey(context, p.EntityRef)
                    : p.KeyValues;

                log.PrimaryKey = JsonSerializer.Serialize(keyValues);
                log.Snapshot   = BuildSnapshot(p, context);

                logs.Add(log);
            }

            // ── Cross-entity audit (e.g. Container → DepartureFlightLog) ─
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
                crossLog.Action     = crossAction;
                crossLog.Timestamp  = now;
                crossLog.CreatedBy  = changedBy;
                crossLog.PrimaryKey = rule.ExtractTargetKey(p.EntityRef);
                crossLog.Snapshot   = BuildSnapshot(p, context);

                logs.Add(crossLog);
            }
        }

        return logs;
    }

    private static string BuildSnapshot(PendingAudit p, DbContext context) =>
        p.State switch
        {
            EntityState.Added => JsonSerializer.Serialize(ResolveCurrentValues(context, p.EntityRef)),
            EntityState.Deleted => JsonSerializer.Serialize(
                p.OriginalValues
                    .Where(kv => AuditDisplayNameRegistry.ResolveIgnore(p.EntityType, kv.Key) == false)
                    .ToDictionary(
                        kv => AuditDisplayNameRegistry.Resolve(p.EntityType, kv.Key),
                        kv => kv.Value)),
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
            changes[displayKey] = new { old = oldVal, @new = newVal };
        }

        return JsonSerializer.Serialize(changes);
    }

    // ── Helpers ───────────────────────────────────────────────────────────
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
                 .Where(kv => AuditDisplayNameRegistry.ResolveIgnore(entityType, kv.Name) == false)
            .ToDictionary(
                p => AuditDisplayNameRegistry.Resolve(entityType, p.Name),
                p => entry.CurrentValues[p]);
    }

    private string ResolveUser() =>
        httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value
        ?? httpContextAccessor.HttpContext?.User.FindFirst("unique_name")?.Value
        ?? "system";

    private static bool IsAuditableEntity(Type type)
    {
        var t = type.BaseType;
        while (t is not null && t != typeof(object))
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(AuditableEntity<>)) return true;
            if (t == typeof(AuditableEntity)) return true;
            t = t.BaseType;
        }
        return false;
    }

    private sealed class PendingAudit
    {
        public Type EntityType { get; }
        public EntityState State { get; }
        public object EntityRef { get; }
        public Dictionary<string, object?> KeyValues { get; }
        public Dictionary<string, object?> OriginalValues { get; }
        public Dictionary<string, object?> CurrentValues { get; }

        public PendingAudit(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            EntityType = entry.Entity.GetType();
            State      = entry.State;
            EntityRef  = entry.Entity;


            KeyValues = entry.Metadata.FindPrimaryKey()?.Properties
                .ToDictionary(
                    p => p.Name,
                    p => entry.CurrentValues[p])
                ?? [];

            OriginalValues = entry.OriginalValues.Properties
                .Where(p => AuditDisplayNameRegistry.ResolveIgnore(EntityType, p.Name) == false)
                .ToDictionary(
                    p => p.Name,
                    p => entry.OriginalValues[p]);

            CurrentValues = entry.State == EntityState.Deleted
                ? OriginalValues
                : entry.CurrentValues.Properties
                    .Where(p => AuditDisplayNameRegistry.ResolveIgnore(EntityType, p.Name) == false)
                    .ToDictionary(
                        p => p.Name,
                        p => entry.CurrentValues[p]);
        }
    }
}
