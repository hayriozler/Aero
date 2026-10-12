using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Flights;
using AeroFlight.Application.Flights.Commands;
using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Identity;
using AeroFlight.Domain.Ops;

namespace AeroFlight.Api.Endpoints.Flights;

public static class FlightFieldEndpoints
{
    public static IEndpointRouteBuilder MapFlightFieldEndpoints(this IEndpointRouteBuilder app)
    {
        MapFieldEndpoints<ArrivalFlight>(app, "/api/flights/arrivals/{flightId:long}");
        MapFieldEndpoints<DepartureFlight>(app, "/api/flights/departures/{flightId:long}");

        return app;
    }

    private static void MapFieldEndpoints<TFlight>(IEndpointRouteBuilder app, string basePath)
        where TFlight : Flight
    {
        app.MapPut($"{basePath}/status", async (
            long flightId,
            UpdateFlightStatusRequest request,
            ICommandHandler<UpdateFlightStatusCommand<TFlight>, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var updated = await handler.HandleAsync(new UpdateFlightStatusCommand<TFlight>(flightId, request.StatusCode), ct);
            return updated
                ? ApiResult.Ok(ctx, true)
                : ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Flight {flightId} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/terminal", async (
            long flightId,
            UpdateFlightTerminalRequest request,
            ICommandHandler<UpdateFlightTerminalCommand<TFlight>, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var updated = await handler.HandleAsync(new UpdateFlightTerminalCommand<TFlight>(flightId, request.TerminalCode), ct);
            return updated
                ? ApiResult.Ok(ctx, true)
                : ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Flight {flightId} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/registration", async (
            long flightId,
            UpdateFlightRegistrationRequest request,
            ICommandHandler<UpdateFlightRegistrationCommand<TFlight>, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var updated = await handler.HandleAsync(new UpdateFlightRegistrationCommand<TFlight>(flightId, request.AircraftCode), ct);
            return updated
                ? ApiResult.Ok(ctx, true)
                : ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Flight {flightId} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);
    }
}
