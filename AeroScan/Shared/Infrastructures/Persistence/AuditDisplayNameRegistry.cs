using System.Text.RegularExpressions;

namespace Infrastructure.Persistence;

public static class AuditDisplayNameRegistry
{
    private static readonly Dictionary<(Type EntityType, string propertyName), string> _map = [];
    private static readonly Dictionary<(Type EntityType, string propertyName), string> _mapIgnore = [];

    internal static void Register(Type entityType, string propertyName, string displayName)
        => _map[(entityType, propertyName)] = displayName;

    internal static void IgnoreRegister(Type entityType, string propertyName)
        => _mapIgnore[(entityType, propertyName)] = propertyName;

    public static string Resolve(Type entityType, string propertyName)
        => _map.TryGetValue((entityType, propertyName), out var auditProperty)
            ? auditProperty
            : SplitPascalCase(propertyName);


    public static bool ResolveIgnore(Type entityType, string propertyName) => _mapIgnore.ContainsKey((entityType, propertyName));

    private static string SplitPascalCase(string name)
        => Regex.Replace(name, "(?<=[a-z])([A-Z])|(?<=[A-Z])([A-Z][a-z])", " $1$2").Trim();
}

