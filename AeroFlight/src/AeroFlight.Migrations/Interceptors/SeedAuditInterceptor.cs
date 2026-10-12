using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AeroFlight.Migrations.Interceptors;

internal sealed class SeedAuditInterceptor : SaveChangesInterceptor
{
    //private static readonly MethodInfo? _setCreate =
    //    typeof(AeroFlight.Domain.Common.AuditBase<>)
    //        .GetMethod("SetAuditOnCreate", BindingFlags.Instance | BindingFlags.NonPublic);

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var now = DateTimeOffset.UtcNow;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added) continue;
            SetAudit(entry.Entity, now);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetAudit(object entity, DateTimeOffset now)
    {
        var type = entity.GetType();

        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Domain.Common.AuditBase<>))
            {
                var method = type.GetMethod("SetAuditOnCreate", BindingFlags.Instance | BindingFlags.NonPublic);
                method?.Invoke(entity, ["seed", now]);
                return;
            }
            type = type.BaseType;
        }
    }
}
