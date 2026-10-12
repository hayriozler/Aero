namespace AeroFlight.Domain.Ops.Resources;

// Pure aggregation over a flight's resource rows (Gate/Belt/Counter), used by ArrivalFlight/
// DepartureFlight's computed Gates/Belts/Counters-style properties: codes joined in plan order,
// the earliest resource's status (deterministic, chronological), and the widest plan window
// (earliest BeginPlan, latest EndPlan) across all of them.
public static class ResourceRollup
{
    public static string? Codes(IEnumerable<ResourceBase> resources)
    {
        var codes = string.Join(" - ", resources
            .OrderBy(r => r.BeginPlan)
            .Select(r => r.Code)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase));

        return codes.Length == 0 ? null : codes;
    }

    public static string? StatusCode(IEnumerable<ResourceBase> resources) =>
        resources.OrderBy(r => r.BeginPlan).Select(r => r.StatusCode).FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

    public static DateTimeOffset? Begin(IEnumerable<ResourceBase> resources) => resources.Min(r => r.BeginPlan);

    public static DateTimeOffset? End(IEnumerable<ResourceBase> resources) => resources.Max(r => r.EndPlan);
}
