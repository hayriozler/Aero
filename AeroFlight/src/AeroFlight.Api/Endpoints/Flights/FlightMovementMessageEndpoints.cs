using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.MovementMessages;
using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Identity;
using AeroFlight.Domain.Ops.MovementMessages;

namespace AeroFlight.Api.Endpoints.Flights;

public static class FlightMovementMessageEndpoints
{
    public static IEndpointRouteBuilder MapFlightMovementMessageEndpoints(this IEndpointRouteBuilder app)
    {
        MapMovementMessageEndpoints<OpsArrivalMovementMessage>(app, "/api/flights/arrivals/{flightId:long}/movement-messages");
        MapMovementMessageEndpoints<OpsDepartureMovementMessage>(app, "/api/flights/departures/{flightId:long}/movement-messages");

        return app;
    }

    private static void MapMovementMessageEndpoints<TMessage>(IEndpointRouteBuilder app, string basePath)
        where TMessage : MovementMessage
    {
        app.MapGet(basePath, async (
            long flightId,
            IQueryHandler<ListMovementMessagesQuery<TMessage>, IReadOnlyList<MovementMessageResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListMovementMessagesQuery<TMessage>(flightId), ct)))
            .RequireAuthorization(Permissions.FlightView);
    }
}
