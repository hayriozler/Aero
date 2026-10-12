using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.Services;
using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Identity;
using AeroFlight.Domain.Ops.Services;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.Flights;

public static class FlightServiceEndpoints
{
    public static IEndpointRouteBuilder MapFlightServiceEndpoints(this IEndpointRouteBuilder app)
    {
        MapServiceEndpoints<OpsArrivalService>(app, "/api/flights/arrivals/{flightId:long}/services");
        MapServiceEndpoints<OpsDepartureService>(app, "/api/flights/departures/{flightId:long}/services");

        return app;
    }

    private static void MapServiceEndpoints<TService>(IEndpointRouteBuilder app, string basePath)
        where TService : OpsService
    {
        app.MapGet(basePath, async (
            long flightId,
            IQueryHandler<ListServicesQuery<TService>, IReadOnlyList<ServiceResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListServicesQuery<TService>(flightId), ct)))
            .RequireAuthorization(Permissions.FlightView);

        app.MapPost(basePath, async (
            long flightId,
            ServiceRequest request,
            ICommandHandler<AddServiceCommand<TService>, ServiceResponse> handler,
            IValidator<ServiceRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new AddServiceCommand<TService>(flightId, request), ct);
            return ApiResult.Created(ctx, result, $"{basePath}/{result.Id}");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/{{serviceId:long}}", async (
            long flightId,
            long serviceId,
            ServiceRequest request,
            ICommandHandler<UpdateServiceCommand<TService>, ServiceResponse?> handler,
            IValidator<ServiceRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateServiceCommand<TService>(flightId, serviceId, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "SERVICE_NOT_FOUND", $"Service {serviceId} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapDelete($"{basePath}/{{serviceId:long}}", async (
            long flightId,
            long serviceId,
            ICommandHandler<DeleteServiceCommand<TService>, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteServiceCommand<TService>(flightId, serviceId), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "SERVICE_NOT_FOUND", $"Service {serviceId} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);
    }
}
