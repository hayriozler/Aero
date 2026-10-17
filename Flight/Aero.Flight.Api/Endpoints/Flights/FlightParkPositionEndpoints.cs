using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Application.Common;
using Aero.Flights.Application.Flights.ParkPositiions;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Identity;
using FluentValidation;

namespace Aero.Flights.Api.Endpoints.Flights;

public static class FlightParkPositionEndpoints
{
    public static IEndpointRouteBuilder MapFlightParkPositionEndpoints(this IEndpointRouteBuilder app)
    {
        MapParkPositionEndpoints(app, "/api/flights/arrivals/{flightId:long}/parkpositions", isArrival: true);
        MapParkPositionEndpoints(app, "/api/flights/departures/{flightId:long}/parkpositions", isArrival: false);

        return app;
    }

    private static void MapParkPositionEndpoints(IEndpointRouteBuilder app, string basePath, bool isArrival)
    {
        app.MapGet(basePath, async (
            long flightId,
            IQueryHandler<ListParkPositionsQuery, IReadOnlyList<OpsParkPositionResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListParkPositionsQuery(flightId, isArrival), ct)))
            .RequireAuthorization(Permissions.FlightView);

        app.MapPost(basePath, async (
            long flightId,
            OpsParkPositionRequest request,
            ICommandHandler<AddParkPositionCommand, OpsParkPositionResponse> handler,
            IValidator<OpsParkPositionRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new AddParkPositionCommand(flightId, isArrival, request), ct);
            return ApiResult.Created(ctx, result, $"{basePath}/{result.Id}");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/{{parkPositionId:long}}", async (
            long flightId,
            long parkPositionId,
            OpsParkPositionRequest request,
            ICommandHandler<UpdateParkPositionCommand, OpsParkPositionResponse?> handler,
            IValidator<OpsParkPositionRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateParkPositionCommand(flightId, isArrival, parkPositionId, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "PARK_POSITION_NOT_FOUND", $"Park position {parkPositionId} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapDelete($"{basePath}/{{parkPositionId:long}}", async (
            long flightId,
            long parkPositionId,
            ICommandHandler<DeleteParkPositionCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteParkPositionCommand(flightId, isArrival, parkPositionId), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "PARK_POSITION_NOT_FOUND", $"Park position {parkPositionId} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);
    }
}
