namespace Aero.Flights.Application.Rules;

// Single place the cache key format is built, shared by RuleEngine (reads/caches) and
// RuleCommandHandlers (evicts on write) — a key typo here would silently break invalidation.
internal static class RuleCacheKeys
{
    public static string Enabled(string tenantId) => $"rules:{tenantId}:enabled";
}
