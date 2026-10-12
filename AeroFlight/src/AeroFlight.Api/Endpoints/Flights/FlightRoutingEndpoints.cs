using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.Routings;
using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Identity;
using AeroFlight.Domain.Ops.Routing;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.Flights;

public static class FlightRoutingEndpoints
{
    public static IEndpointRouteBuilder MapFlightRoutingEndpoints(this IEndpointRouteBuilder app)
    {
        MapRoutingEndpoints<OpsArrivalRouting>(app, "/api/flights/arrivals/{flightId:long}/routings");
        MapRoutingEndpoints<OpsDepartureRouting>(app, "/api/flights/departures/{flightId:long}/routings");

        return app;
    }

    private static void MapRoutingEndpoints<TRouting>(IEndpointRouteBuilder app, string basePath)
        where TRouting : OpsRouting
    {
        app.MapGet(basePath, async (
            long flightId,
            IQueryHandler<ListRoutingsQuery<TRouting>, IReadOnlyList<OpsRoutingResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListRoutingsQuery<TRouting>(flightId), ct)))
            .RequireAuthorization(Permissions.FlightView);

        app.MapPost(basePath, async (
            long flightId,
            OpsRoutingRequest request,
            ICommandHandler<AddRoutingCommand<TRouting>, AddRoutingResult> handler,
            IValidator<OpsRoutingRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new AddRoutingCommand<TRouting>(flightId, request), ct);
            return ApiResult.Created(ctx, result.Routing!, $"{basePath}/{result.Routing!.Id}");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/{{routingId:long}}", async (
            long flightId,
            long routingId,
            OpsRoutingRequest request,
            ICommandHandler<UpdateRoutingCommand<TRouting>, OpsRoutingResponse?> handler,
            IValidator<OpsRoutingRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateRoutingCommand<TRouting>(flightId, routingId, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "ROUTING_NOT_FOUND", $"OpsRouting {routingId} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapDelete($"{basePath}/{{routingId:long}}", async (
            long flightId,
            long routingId,
            ICommandHandler<DeleteRoutingCommand<TRouting>, DeleteRoutingOutcome> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var outcome = await handler.HandleAsync(new DeleteRoutingCommand<TRouting>(flightId, routingId), ct);
            return outcome switch
            {
                DeleteRoutingOutcome.Deleted => ApiResult.NoContent(ctx),
                DeleteRoutingOutcome.NotFound => ApiResult.NotFound(ctx, "ROUTING_NOT_FOUND", $"OpsRouting {routingId} not found."),
                _ => ApiResult.Conflict(ctx, "LAST_ROUTING_LEG", "Cannot delete the only remaining routing leg."),
            };
        }).RequireAuthorization(Permissions.FlightWrite);
    }
}
