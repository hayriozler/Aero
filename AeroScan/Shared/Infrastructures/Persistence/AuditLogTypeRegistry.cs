namespace Infrastructure.Persistence;

public static class AuditLogTypeRegistry
{
    private static readonly Dictionary<Type, Type> _map = [];

    internal static void Register(Type entityType, Type logType) =>
        _map[entityType] = logType;

    public static bool TryGet(Type entityType, out Type? logType) =>
        _map.TryGetValue(entityType, out logType);

    public static IReadOnlyDictionary<Type, Type> All => _map;
}