using System.Text.RegularExpressions;

namespace Aero.Shared.Infrastructure.Extensions;

internal static class AuditDisplayNameRegistry
{
    private static readonly Dictionary<(Type, string), string> _displayNames = [];
    private static readonly Dictionary<(Type, string), bool> _ignored = [];

    internal static void Register(Type entityType, string propertyName, string displayName) =>
        _displayNames[(entityType, propertyName)] = displayName;

    internal static void RegisterIgnore(Type entityType, string propertyName) =>
        _ignored[(entityType, propertyName)] = true;

    internal static string Resolve(Type entityType, string propertyName) =>
        _displayNames.TryGetValue((entityType, propertyName), out var name)
            ? name
            : SplitPascalCase(propertyName);

    internal static bool IsIgnored(Type entityType, string propertyName) =>
        _ignored.ContainsKey((entityType, propertyName));

    private static string SplitPascalCase(string name) =>
        Regex.Replace(name, "(?<=[a-z])([A-Z])|(?<=[A-Z])([A-Z][a-z])", " $1$2").Trim();
}
