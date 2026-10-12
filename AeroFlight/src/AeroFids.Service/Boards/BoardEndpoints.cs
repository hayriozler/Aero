using AeroFids.Infrastructure.Boards;
using AeroFids.Infrastructure.Flights;
using AeroFlight.Contracts.Fids;

namespace AeroFids.Service.Boards;

public static class BoardEndpoints
{
    public static IEndpointRouteBuilder MapBoardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHub<BoardHub>("/hubs/board");

        return app;
    }

    // resourceCode is the connecting DisplayClient's own ResourceCode — required (checked by the
    // caller, BoardHub) for every resource Type; unused for flight-list types.
    internal static async Task<object> BuildBoardAsync(
        string tenant, string type, string? resourceCode, IFlightBoardStore store, IFlightBoardBootstrapper bootstrapper,
        IFieldMappingRegistry fieldMappings, CancellationToken ct)
    {
        switch (type)
        {
            case DisplayClientTypes.DepartureFlightList:
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Departure, ct);
                return store.GetBoard(tenant, FlightTypes.Departure);

            case DisplayClientTypes.ArrivalFlightList:
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Arrival, ct);
                return store.GetBoard(tenant, FlightTypes.Arrival);

            case DisplayClientTypes.FullFlightList:
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Arrival, ct);
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Departure, ct);
                return store.GetAll(tenant);

            // Gates and spots apply to both directions. resourceCode null means this client shows
            // every code of this kind (an overview board) instead of one physical instance.
            case DisplayClientTypes.Gate:
            {
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Arrival, ct);
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Departure, ct);
                var mapper = await fieldMappings.GetMapperAsync(tenant, ct);
                Func<string?, bool> isShown = s => mapper.IsResourceShown(ResourceKinds.Gate, s);
                return resourceCode is null
                    ? ResourceDatasetProjector.ProjectOverview(ResourceKinds.Gate, store.GetAll(tenant), isShown)
                    : ResourceDatasetProjector.ProjectForCode(ResourceKinds.Gate, resourceCode, store.GetAll(tenant), isShown);
            }

            case DisplayClientTypes.Spot:
            {
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Arrival, ct);
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Departure, ct);
                var mapper = await fieldMappings.GetMapperAsync(tenant, ct);
                Func<string?, bool> isShown = s => mapper.IsResourceShown(ResourceKinds.Spot, s);
                return resourceCode is null
                    ? ResourceDatasetProjector.ProjectOverview(ResourceKinds.Spot, store.GetAll(tenant), isShown)
                    : ResourceDatasetProjector.ProjectForCode(ResourceKinds.Spot, resourceCode, store.GetAll(tenant), isShown);
            }

            // Counters only ever come from departure flights (see FlightFieldMapper).
            case DisplayClientTypes.Counter:
            {
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Departure, ct);
                var mapper = await fieldMappings.GetMapperAsync(tenant, ct);
                Func<string?, bool> isShown = s => mapper.IsResourceShown(ResourceKinds.Counter, s);
                var flights = store.GetBoard(tenant, FlightTypes.Departure);
                return resourceCode is null
                    ? ResourceDatasetProjector.ProjectOverview(ResourceKinds.Counter, flights, isShown)
                    : ResourceDatasetProjector.ProjectForCode(ResourceKinds.Counter, resourceCode, flights, isShown);
            }

            case DisplayClientTypes.ArrivalBelt:
            {
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Arrival, ct);
                var mapper = await fieldMappings.GetMapperAsync(tenant, ct);
                Func<string?, bool> isShown = s => mapper.IsResourceShown(ResourceKinds.Belt, s);
                var flights = store.GetBoard(tenant, FlightTypes.Arrival);
                return resourceCode is null
                    ? ResourceDatasetProjector.ProjectOverview(ResourceKinds.Belt, flights, isShown)
                    : ResourceDatasetProjector.ProjectForCode(ResourceKinds.Belt, resourceCode, flights, isShown);
            }

            case DisplayClientTypes.DepartureBelt:
            {
                await bootstrapper.EnsureBootstrappedAsync(tenant, FlightTypes.Departure, ct);
                var mapper = await fieldMappings.GetMapperAsync(tenant, ct);
                Func<string?, bool> isShown = s => mapper.IsResourceShown(ResourceKinds.Belt, s);
                var flights = store.GetBoard(tenant, FlightTypes.Departure);
                return resourceCode is null
                    ? ResourceDatasetProjector.ProjectOverview(ResourceKinds.Belt, flights, isShown)
                    : ResourceDatasetProjector.ProjectForCode(ResourceKinds.Belt, resourceCode, flights, isShown);
            }

            default:
                return Array.Empty<object>();
        }
    }
}
