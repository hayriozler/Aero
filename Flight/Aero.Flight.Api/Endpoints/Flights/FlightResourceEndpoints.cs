using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Application.Common;
using Aero.Flights.Application.Configuration;
using Aero.Flights.Application.Flights.Resources;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Identity;
using Aero.Flights.Domain.Ops.Resources;
using FluentValidation;

namespace Aero.Flights.Api.Endpoints.Flights;

public static class FlightResourceEndpoints
{
    private static readonly Dictionary<string, string> _defaultDurationConfigNames = new(StringComparer.OrdinalIgnoreCase)
    {
        // "gate" is a leg-agnostic fallback for ResourceTimeline.razor's unified gate browser,
        // which isn't tied to one flight/leg — mapped to the arrival figures. Leg-specific callers
        // (e.g. FlightEditPanel.razor) use "arrivalgate"/"departuregate" for the precise default.
        ["gate"] = "Resources.ArrivalGate.DefaultDurationMinutes",
        ["arrivalgate"] = "Resources.ArrivalGate.DefaultDurationMinutes",
        ["departuregate"] = "Resources.DepartureGate.DefaultDurationMinutes",
        ["arrivalbelt"] = "Resources.ArrivalBelt.DefaultDurationMinutes",
        ["departurebelt"] = "Resources.DepartureBelt.DefaultDurationMinutes",
        ["counter"] = "Resources.Counter.DefaultDurationMinutes",
        ["parkposition"] = "Resources.ParkPosition.DefaultDurationMinutes",
    };

    public static IEndpointRouteBuilder MapFlightResourceEndpoints(this IEndpointRouteBuilder app)
    {
        MapResourceEndpoints<OpsArrivalGate>(app, "/api/flights/arrivals/{flightId:long}/gates");
        MapResourceEndpoints<OpsArrivalBelt>(app, "/api/flights/arrivals/{flightId:long}/arrivalbelts");
        MapResourceEndpoints<OpsDepartureGate>(app, "/api/flights/departures/{flightId:long}/gates");
        MapResourceEndpoints<OpsDepartureBelt>(app, "/api/flights/departures/{flightId:long}/departurebelts");
        MapResourceEndpoints<OpsCounter>(app, "/api/flights/departures/{flightId:long}/counters");

        app.MapGet("/api/flights/resource-defaults/{kind}", async (
            string kind,
            IAeroConfigService aeroConfig,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            if (!_defaultDurationConfigNames.TryGetValue(kind, out var configName))
                return ApiResult.NotFound(ctx, "UNKNOWN_RESOURCE_KIND", $"Unknown resource kind '{kind}'.");

            var minutes = await aeroConfig.GetIntAsync(configName, ResourceDefaultsConstants.FallbackDurationMinutes, ct);
            return ApiResult.Ok(ctx, new ResourceDefaultsResponse(minutes));
        }).RequireAuthorization(Permissions.FlightView);

        app.MapGet("/api/flights/status-colors", async (
            IAeroConfigService aeroConfig,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var colors = await aeroConfig.GetByPrefixAsync("FlightStatus.Color.", ct);
            return ApiResult.Ok(ctx, colors);
        }).RequireAuthorization(Permissions.FlightView);

        app.MapGet("/api/flights/resource-grid-date-format", async (
            IAeroConfigService aeroConfig,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var format = await aeroConfig.GetAsync("Resources.Grid.DateFormat", ct);
            return ApiResult.Ok(ctx, new ResourceGridDateFormatResponse(format ?? "dd.MM"));
        }).RequireAuthorization(Permissions.FlightView);

        return app;
    }

    private sealed record ResourceDefaultsResponse(int DurationMinutes);
    private sealed record ResourceGridDateFormatResponse(string DateFormat);

    private static void MapResourceEndpoints<TResource>(IEndpointRouteBuilder app, string basePath)
        where TResource : ResourceBase
    {
        app.MapGet(basePath, async (
            long flightId,
            IQueryHandler<ListFlightResourcesQuery<TResource>, IReadOnlyList<FlightResourceResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListFlightResourcesQuery<TResource>(flightId), ct)))
            .RequireAuthorization(Permissions.FlightView);

        app.MapPost(basePath, async (
            long flightId,
            FlightResourceRequest request,
            ICommandHandler<AddFlightResourceCommand<TResource>, FlightResourceResponse> handler,
            IValidator<FlightResourceRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new AddFlightResourceCommand<TResource>(flightId, request), ct);
            return ApiResult.Created(ctx, result, $"{basePath}/{result.Id}");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/{{resourceId:long}}", async (
            long flightId,
            long resourceId,
            FlightResourceRequest request,
            ICommandHandler<UpdateFlightResourceCommand<TResource>, FlightResourceResponse?> handler,
            IValidator<FlightResourceRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateFlightResourceCommand<TResource>(flightId, resourceId, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "RESOURCE_NOT_FOUND", $"Resource {resourceId} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapDelete($"{basePath}/{{resourceId:long}}", async (
            long flightId,
            long resourceId,
            ICommandHandler<DeleteFlightResourceCommand<TResource>, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteFlightResourceCommand<TResource>(flightId, resourceId), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "RESOURCE_NOT_FOUND", $"Resource {resourceId} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/status", async (
            long flightId,
            BulkResourceStatusRequest request,
            ICommandHandler<BulkUpdateResourceStatusCommand<TResource>, IReadOnlyList<FlightResourceResponse>> handler,
            IValidator<BulkResourceStatusRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new BulkUpdateResourceStatusCommand<TResource>(flightId, request.StatusCode), ct);
            return ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.FlightWrite);
    }
}
