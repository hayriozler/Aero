namespace AeroFlight.Infrastructure.Persistence.Audit;

internal static class AuditLogTypeRegistry
{
    private static readonly Dictionary<Type, Type> _map = [];

    internal static void Register(Type entityType, Type logType) =>
        _map[entityType] = logType;

    internal static bool TryGet(Type entityType, out Type? logType) =>
        _map.TryGetValue(entityType, out logType);

    internal static IReadOnlyDictionary<Type, Type> All => _map;
}
