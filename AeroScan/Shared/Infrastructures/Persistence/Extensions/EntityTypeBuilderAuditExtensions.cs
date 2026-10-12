using Domain.Audits;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;
using System.Text.Json;

namespace Infrastructure.Persistence.Extensions;

public static class EntityTypeBuilderAuditExtensions
{
    public static EntityTypeBuilder<T> HasAuditDisplayName<T, TProp>(
        this EntityTypeBuilder<T> builder,
        Expression<Func<T, TProp>> property,
        string displayName) where T : class
    {
        if (property.Body is MemberExpression member)
            AuditDisplayNameRegistry.Register(typeof(T), member.Member.Name, displayName);

        return builder;
    }
    public static EntityTypeBuilder<T> IgnoreAudit<T, TProp>(
        this EntityTypeBuilder<T> builder,
        Expression<Func<T, TProp>> property) where T : class
    {
        if (property.Body is MemberExpression member)
            AuditDisplayNameRegistry.IgnoreRegister(typeof(T), member.Member.Name);

        return builder;
    }

    public static EntityTypeBuilder<T> HasAuditType<T, TLog>(
        this EntityTypeBuilder<T> builder)
        where T : class
        where TLog : AuditLogBase
    {
        AuditLogTypeRegistry.Register(typeof(T), typeof(TLog));
        return builder;
    }

    public static EntityTypeBuilder<T> HasCrossAudit<T, TRelatedLog>(
        this EntityTypeBuilder<T> builder,
        Func<T, int> relatedKeySelector,
        string actionPrefix)
        where T : class
        where TRelatedLog : AuditLogBase
    {
        CrossAuditRegistry.Register(typeof(T), new CrossAuditRegistry.Rule(
            TargetLogType: typeof(TRelatedLog),
            ExtractTargetKey: entity => JsonSerializer.Serialize(
                new Dictionary<string, object?> { ["Id"] = relatedKeySelector((T)entity) }),
            ActionPrefix: actionPrefix));

        return builder;
    }   
}
