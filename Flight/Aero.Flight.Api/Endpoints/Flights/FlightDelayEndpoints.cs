using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Application.Common;
using Aero.Flights.Application.Flights.Delays;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Identity;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Ops.Delays;
using FluentValidation;

namespace Aero.Flights.Api.Endpoints.Flights;

public static class FlightDelayEndpoints
{
    public static IEndpointRouteBuilder MapFlightDelayEndpoints(this IEndpointRouteBuilder app)
    {
        MapDelayEndpoints<OpsArrivalDelay>(app, "/api/flights/arrivals/{flightId:long}/delays");
        MapDelayEndpoints<OpsDepartureDelay>(app, "/api/flights/departures/{flightId:long}/delays");

        return app;
    }

    private static void MapDelayEndpoints<TDelay>(IEndpointRouteBuilder app, string basePath)
        where TDelay : Delay
    {
        app.MapGet(basePath, async (
            long flightId,
            IQueryHandler<ListDelaysQuery<TDelay>, IReadOnlyList<DelayResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListDelaysQuery<TDelay>(flightId), ct)))
            .RequireAuthorization(Permissions.FlightView);

        app.MapPost(basePath, async (
            long flightId,
            DelayRequest request,
            ICommandHandler<AddDelayCommand<TDelay>, DelayResponse> handler,
            IValidator<DelayRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new AddDelayCommand<TDelay>(flightId, request), ct);
            return ApiResult.Created(ctx, result, $"{basePath}/{result.Id}");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/{{delayId:long}}", async (
            long flightId,
            long delayId,
            DelayRequest request,
            ICommandHandler<UpdateDelayCommand<TDelay>, DelayResponse?> handler,
            IValidator<DelayRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateDelayCommand<TDelay>(flightId, delayId, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "DELAY_NOT_FOUND", $"Delay {delayId} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapDelete($"{basePath}/{{delayId:long}}", async (
            long flightId,
            long delayId,
            ICommandHandler<DeleteDelayCommand<TDelay>, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteDelayCommand<TDelay>(flightId, delayId), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "DELAY_NOT_FOUND", $"Delay {delayId} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);
    }
}
