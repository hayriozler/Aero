using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.SeasonalFlightSchedules;
using AeroFlight.Contracts.SeasonalFlightSchedules;
using AeroFlight.Domain.Identity;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.SeasonalFlightSchedules;

public static class SeasonalFlightScheduleEndpoints
{
    public static IEndpointRouteBuilder MapSeasonalFlightScheduleEndpoints(this IEndpointRouteBuilder app)
    {
        MapSeasonEndpoints(app);
        MapArrivalFlightEndpoints(app);
        MapDepartureFlightEndpoints(app);
        return app;
    }

    private static void MapSeasonEndpoints(IEndpointRouteBuilder app)
    {
        const string basePath = "/api/seasonal-flight-schedule/seasons";

        app.MapGet(basePath, async (
            IQueryHandler<ListScheduleSeasonsQuery, IReadOnlyList<ScheduleSeasonResponse>> handler,
            HttpContext ctx, CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListScheduleSeasonsQuery(), ct)))
            .RequireAuthorization(Permissions.SeasonalFlightScheduleView);

        app.MapGet($"{basePath}/{{code}}", async (
            string code,
            IQueryHandler<GetScheduleSeasonQuery, ScheduleSeasonResponse?> handler,
            HttpContext ctx, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetScheduleSeasonQuery(code), ct);
            return result is null ? ApiResult.NotFound(ctx, "SEASON_NOT_FOUND", $"Season '{code}' not found.") : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleView);

        app.MapPost(basePath, async (
            ScheduleSeasonRequest request,
            ICommandHandler<CreateScheduleSeasonCommand, ScheduleSeasonResponse> handler,
            IValidator<ScheduleSeasonRequest> validator,
            HttpContext ctx, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new CreateScheduleSeasonCommand(request), ct);
            return ApiResult.Created(ctx, result, $"{basePath}/{result.Code}");
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);

        app.MapPut($"{basePath}/{{code}}", async (
            string code,
            ScheduleSeasonRequest request,
            ICommandHandler<UpdateScheduleSeasonCommand, ScheduleSeasonResponse?> handler,
            IValidator<ScheduleSeasonRequest> validator,
            HttpContext ctx, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateScheduleSeasonCommand(code, request), ct);
            return result is null ? ApiResult.NotFound(ctx, "SEASON_NOT_FOUND", $"Season '{code}' not found.") : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);

        app.MapDelete($"{basePath}/{{code}}", async (
            string code,
            ICommandHandler<DeleteScheduleSeasonCommand, bool> handler,
            HttpContext ctx, CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteScheduleSeasonCommand(code), ct);
            return deleted ? ApiResult.NoContent(ctx) : ApiResult.NotFound(ctx, "SEASON_NOT_FOUND", $"Season '{code}' not found.");
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);
    }

    private static void MapArrivalFlightEndpoints(IEndpointRouteBuilder app)
    {
        const string basePath = "/api/seasonal-flight-schedule/arrivals";

        app.MapGet(basePath, async (
            string? scheduleSeasonCode,
            IQueryHandler<ListSeasonalFlightScheduleArrivalFlightsQuery, IReadOnlyList<SeasonalFlightScheduleArrivalFlightResponse>> handler,
            HttpContext ctx, CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListSeasonalFlightScheduleArrivalFlightsQuery(scheduleSeasonCode), ct)))
            .RequireAuthorization(Permissions.SeasonalFlightScheduleView);

        app.MapGet($"{basePath}/{{id:int}}", async (
            int id,
            IQueryHandler<GetSeasonalFlightScheduleArrivalFlightQuery, SeasonalFlightScheduleArrivalFlightResponse?> handler,
            HttpContext ctx, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetSeasonalFlightScheduleArrivalFlightQuery(id), ct);
            return result is null ? ApiResult.NotFound(ctx, "SCHEDULE_FLIGHT_NOT_FOUND", $"Arrival schedule flight {id} not found.") : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleView);

        app.MapPost(basePath, async (
            SeasonalFlightScheduleArrivalFlightRequest request,
            ICommandHandler<CreateSeasonalFlightScheduleArrivalFlightCommand, SeasonalFlightScheduleArrivalFlightResponse> handler,
            IValidator<SeasonalFlightScheduleArrivalFlightRequest> validator,
            HttpContext ctx, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new CreateSeasonalFlightScheduleArrivalFlightCommand(request), ct);
            return ApiResult.Created(ctx, result, $"{basePath}/{result.Id}");
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);

        app.MapPut($"{basePath}/{{id:int}}", async (
            int id,
            SeasonalFlightScheduleArrivalFlightRequest request,
            ICommandHandler<UpdateSeasonalFlightScheduleArrivalFlightCommand, SeasonalFlightScheduleArrivalFlightResponse?> handler,
            IValidator<SeasonalFlightScheduleArrivalFlightRequest> validator,
            HttpContext ctx, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateSeasonalFlightScheduleArrivalFlightCommand(id, request), ct);
            return result is null ? ApiResult.NotFound(ctx, "SCHEDULE_FLIGHT_NOT_FOUND", $"Arrival schedule flight {id} not found.") : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);

        app.MapDelete($"{basePath}/{{id:int}}", async (
            int id,
            ICommandHandler<DeleteSeasonalFlightScheduleArrivalFlightCommand, bool> handler,
            HttpContext ctx, CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteSeasonalFlightScheduleArrivalFlightCommand(id), ct);
            return deleted ? ApiResult.NoContent(ctx) : ApiResult.NotFound(ctx, "SCHEDULE_FLIGHT_NOT_FOUND", $"Arrival schedule flight {id} not found.");
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);
    }

    private static void MapDepartureFlightEndpoints(IEndpointRouteBuilder app)
    {
        const string basePath = "/api/seasonal-flight-schedule/departures";

        app.MapGet(basePath, async (
            string? scheduleSeasonCode,
            IQueryHandler<ListSeasonalFlightScheduleDepartureFlightsQuery, IReadOnlyList<SeasonalFlightScheduleDepartureFlightResponse>> handler,
            HttpContext ctx, CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListSeasonalFlightScheduleDepartureFlightsQuery(scheduleSeasonCode), ct)))
            .RequireAuthorization(Permissions.SeasonalFlightScheduleView);

        app.MapGet($"{basePath}/{{id:int}}", async (
            int id,
            IQueryHandler<GetSeasonalFlightScheduleDepartureFlightQuery, SeasonalFlightScheduleDepartureFlightResponse?> handler,
            HttpContext ctx, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetSeasonalFlightScheduleDepartureFlightQuery(id), ct);
            return result is null ? ApiResult.NotFound(ctx, "SCHEDULE_FLIGHT_NOT_FOUND", $"Departure schedule flight {id} not found.") : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleView);

        app.MapPost(basePath, async (
            SeasonalFlightScheduleDepartureFlightRequest request,
            ICommandHandler<CreateSeasonalFlightScheduleDepartureFlightCommand, SeasonalFlightScheduleDepartureFlightResponse> handler,
            IValidator<SeasonalFlightScheduleDepartureFlightRequest> validator,
            HttpContext ctx, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new CreateSeasonalFlightScheduleDepartureFlightCommand(request), ct);
            return ApiResult.Created(ctx, result, $"{basePath}/{result.Id}");
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);

        app.MapPut($"{basePath}/{{id:int}}", async (
            int id,
            SeasonalFlightScheduleDepartureFlightRequest request,
            ICommandHandler<UpdateSeasonalFlightScheduleDepartureFlightCommand, SeasonalFlightScheduleDepartureFlightResponse?> handler,
            IValidator<SeasonalFlightScheduleDepartureFlightRequest> validator,
            HttpContext ctx, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateSeasonalFlightScheduleDepartureFlightCommand(id, request), ct);
            return result is null ? ApiResult.NotFound(ctx, "SCHEDULE_FLIGHT_NOT_FOUND", $"Departure schedule flight {id} not found.") : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);

        app.MapDelete($"{basePath}/{{id:int}}", async (
            int id,
            ICommandHandler<DeleteSeasonalFlightScheduleDepartureFlightCommand, bool> handler,
            HttpContext ctx, CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteSeasonalFlightScheduleDepartureFlightCommand(id), ct);
            return deleted ? ApiResult.NoContent(ctx) : ApiResult.NotFound(ctx, "SCHEDULE_FLIGHT_NOT_FOUND", $"Departure schedule flight {id} not found.");
        }).RequireAuthorization(Permissions.SeasonalFlightScheduleWrite);
    }
}
