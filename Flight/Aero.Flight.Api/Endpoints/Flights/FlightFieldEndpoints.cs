using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Application.Common;
using Aero.Flights.Application.Flights.Commands;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Identity;
using Aero.Flights.Domain.Ops;
using AeroFlight.Application.Flights;

namespace Aero.Flights.Api.Endpoints.Flights;

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
