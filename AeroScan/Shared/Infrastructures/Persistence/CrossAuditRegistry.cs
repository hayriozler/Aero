using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Persistence;

public static class CrossAuditRegistry
{
    public sealed record Rule(
        Type TargetLogType,
        Func<object, string> ExtractTargetKey,
        string ActionPrefix);

    private static readonly Dictionary<Type, Rule> _rules = [];

    internal static void Register(Type entityType, Rule rule) =>
        _rules[entityType] = rule;

    public static bool TryGet(Type entityType, [NotNullWhen(true)] out Rule? rule) =>
        _rules.TryGetValue(entityType, out rule);
}
