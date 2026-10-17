using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Application.Common;
using Aero.Flights.Application.Resources;
using Aero.Flights.Domain.Identity;

namespace Aero.Flights.Api.Endpoints.Resources;

public static class ResourceEndpoints
{
    private static readonly Dictionary<string, string> _typesByRoute = new(StringComparer.OrdinalIgnoreCase)
    {
        ["gates"] = "Gate",
        ["counters"] = "Counter",
        ["parkpositions"] = "ParkPosition",
        // Arrival and departure belts are separate physical resources tracked in separate tables —
        // the same belt code on each side is not the same belt, so they can't share a Gantt row.
        ["arrival-belts"] = "ArrivalBelt",
        ["departure-belts"] = "DepartureBelt",
    };

    public static IEndpointRouteBuilder MapResourceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/resources")
            .WithTags("Resources")
            .RequireAuthorization(Permissions.FlightView);

        group.MapGet("/{type}/timeline", async (
            string type,
            DateOnly fromDate,
            DateOnly toDate,
            IQueryHandler<GetGanttItemsQuery, IReadOnlyList<GanttItem>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            if (!_typesByRoute.TryGetValue(type, out var resourceType))
                return ApiResult.NotFound(ctx, "RESOURCE_TYPE_NOT_FOUND", $"Unknown resource type '{type}'.");

            var result = await handler.HandleAsync(new GetGanttItemsQuery(resourceType, fromDate, toDate), ct);
            return ApiResult.Ok(ctx, result);
        });

        group.MapGet("/{type}/unassigned", async (
            string type,
            DateOnly fromDate,
            DateOnly toDate,
            IQueryHandler<GetUnassignedFlightsQuery, IReadOnlyList<UnassignedFlightItem>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            if (!_typesByRoute.TryGetValue(type, out var resourceType))
                return ApiResult.NotFound(ctx, "RESOURCE_TYPE_NOT_FOUND", $"Unknown resource type '{type}'.");

            var result = await handler.HandleAsync(new GetUnassignedFlightsQuery(resourceType, fromDate, toDate), ct);
            return ApiResult.Ok(ctx, result);
        });

        return app;
    }
}
