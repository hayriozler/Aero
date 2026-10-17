using Aero.Fids.Infrastructure.Flights;

namespace Aero.Fids.Web.Boards;

// Projects a FidsFlight down to the trimmed dataset item a resource-type client (Gate/Belt/Counter/
// Spot) actually receives — used both for the initial snapshot (BoardEndpoints) and for diffing old
// vs new state to decide whether a push is warranted (FlightUpdatesConsumer).
internal static class ResourceDatasetProjector
{
    public static string EventNameFor(string kind) => kind switch
    {
        ResourceKinds.Gate => "gateUpdated",
        ResourceKinds.Belt => "beltUpdated",
        ResourceKinds.Counter => "counterUpdated",
        ResourceKinds.Spot => "spotUpdated",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown resource kind."),
    };

    public static IReadOnlyList<object> ProjectForCode(string kind, string code, IReadOnlyList<FidsFlight> flights, Func<string?, bool> isShown) =>
        [.. flights.SelectMany(f => f.Resources
            .Where(r => string.Equals(r.Kind, kind, StringComparison.OrdinalIgnoreCase) && string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase) && isShown(r.ExternalStatus))
            .Select(r => Project(f, kind, r)))];

    public static object? ProjectOne(FidsFlight? flight, string kind, string code, Func<string?, bool> isShown)
    {
        var resource = flight?.Resources.FirstOrDefault(r =>
            string.Equals(r.Kind, kind, StringComparison.OrdinalIgnoreCase) && string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase));
        return resource is null || !isShown(resource.ExternalStatus) ? null : Project(flight!, kind, resource);
    }

    public static IReadOnlyList<ResourceOverviewItem> ProjectOverview(string kind, IReadOnlyList<FidsFlight> flights, Func<string?, bool> isShown)
    {
        var byCode = new Dictionary<string, (string? Status, List<ResourceOverviewFlight> Flights)>(StringComparer.OrdinalIgnoreCase);

        foreach (var flight in flights)
        {
            foreach (var resource in flight.Resources)
            {
                if (!string.Equals(resource.Kind, kind, StringComparison.OrdinalIgnoreCase) || !isShown(resource.ExternalStatus))
                    continue;

                if (!byCode.TryGetValue(resource.Code, out var entry))
                    entry = (null, []);

                entry.Flights.Add(new ResourceOverviewFlight(
                    flight.FlightId, flight.Designators, flight.Status,
                    flight.ScheduledTime, flight.ScheduledTimeLocal, flight.EstimatedTime, flight.EstimatedTimeLocal));
                byCode[resource.Code] = (entry.Status ?? resource.Status, entry.Flights);
            }
        }

        return [.. byCode
            .Select(kv => new ResourceOverviewItem(kv.Key, kv.Value.Status, kv.Value.Flights))
            .OrderBy(i => i.Code, StringComparer.OrdinalIgnoreCase)];
    }

    private static object Project(FidsFlight flight, string kind, FidsResource resource) => kind switch
    {
        ResourceKinds.Gate => new GateDataSetItem(
            flight.FlightId, flight.FlightType, flight.Designators, flight.Status,
            flight.ScheduledTime, flight.ScheduledTimeLocal, flight.EstimatedTime, flight.EstimatedTimeLocal, resource.Code, resource.Status),
        ResourceKinds.Belt => new BeltDataSetItem(
            flight.FlightId, flight.FlightType, flight.Designators, flight.Status,
            flight.ScheduledTime, flight.ScheduledTimeLocal, flight.EstimatedTime, flight.EstimatedTimeLocal, resource.Code, resource.Status),
        ResourceKinds.Counter => new CounterDataSetItem(
            flight.FlightId, flight.FlightType, flight.Designators, flight.Status,
            flight.ScheduledTime, flight.ScheduledTimeLocal, flight.EstimatedTime, flight.EstimatedTimeLocal, resource.Code, resource.Status, resource.DisplayCode),
        ResourceKinds.Spot => new SpotDataSetItem(
            flight.FlightId, flight.FlightType, flight.Designators, flight.Status,
            flight.ScheduledTime, flight.ScheduledTimeLocal, flight.EstimatedTime, flight.EstimatedTimeLocal, resource.Code, resource.Status),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown resource kind."),
    };
}
