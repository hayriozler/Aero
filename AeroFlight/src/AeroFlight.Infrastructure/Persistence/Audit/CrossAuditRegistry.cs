using System.Diagnostics.CodeAnalysis;

namespace AeroFlight.Infrastructure.Persistence.Audit;

internal static class CrossAuditRegistry
{
    private static readonly Dictionary<Type, AuditRule> _rules = [];

    internal static void Register(Type entityType, AuditRule rule) =>
        _rules[entityType] = rule;

    internal static bool TryGet(Type entityType, [NotNullWhen(true)] out AuditRule? rule) =>
        _rules.TryGetValue(entityType, out rule);
}
