using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.Commands;
using AeroFlight.Application.Flights.Queries;
using AeroFlight.Contracts.Flights;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Identity;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.Flights;

public static class FlightEndpoints
{
    public static IEndpointRouteBuilder MapFlightEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/flights")
            .WithTags("Flights")
            .RequireAuthorization(Permissions.FlightView);

        group.MapGet("/", async (
            DateOnly? fromDate,
            DateOnly? toDate,
            string[]? direction,
            string? airlineCode,
            string? search,
            string[]? terminalCode,
            string[]? countryType,
            string[]? statuses,
            string[]? flightType,
            int page,
            int? pageSize,
            IQueryHandler<GetFlightsQuery, PagedResult<MergedFlightRow>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var query = new GetFlightsQuery(fromDate, toDate, direction, airlineCode, search, terminalCode, countryType, statuses, flightType, page < 1 ? 1 : page, pageSize);
            var result = await handler.HandleAsync(query, ct);
            var totalPages = result.PageSize == 0 ? 0 : (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);
            return ApiResult.Paged(ctx, result.Items, new ApiPagination(result.Page, result.PageSize, result.TotalCount, totalPages));
        });

        group.MapGet("/ops-overview", async (
            DateOnly date,
            IQueryHandler<GetOpsOverviewQuery, OpsOverview> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetOpsOverviewQuery(date), ct)));

        group.MapGet("/arrivals/{id:long}", async (
            long id,
            IQueryHandler<GetArrivalFlightDetailQuery, ArrivalFlightDetail?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetArrivalFlightDetailQuery(id), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Arrival flight {id} not found.")
                : ApiResult.Ok(ctx, result);
        });

        group.MapGet("/departures/{id:long}", async (
            long id,
            IQueryHandler<GetDepartureFlightDetailQuery, DepartureFlightDetail?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetDepartureFlightDetailQuery(id), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Departure flight {id} not found.")
                : ApiResult.Ok(ctx, result);
        });

        group.MapGet("/arrivals/{id:long}/audit", async (
            long id,
            IQueryHandler<GetFlightFullAuditQuery, IReadOnlyList<AuditEntryResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetFlightFullAuditQuery(id, IsArrival: true), ct)));

        group.MapGet("/departures/{id:long}/audit", async (
            long id,
            IQueryHandler<GetFlightFullAuditQuery, IReadOnlyList<AuditEntryResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetFlightFullAuditQuery(id, IsArrival: false), ct)));

        group.MapPost("/arrivals", async (
            ArrivalCreateRequest request,
            ICommandHandler<CreateArrivalFlightCommand, CreateFlightResult> handler,
            IValidator<ArrivalCreateRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new CreateArrivalFlightCommand(request), ct);
            return result.Outcome switch
            {
                CreateFlightOutcome.DuplicateFlight => ApiResult.Conflict(ctx, "DUPLICATE_FLIGHT",
                    "A flight with this airline, flight number and date already exists."),
                _ => ApiResult.Created(ctx, result.Flight!, $"/api/flights/{result.Flight!.Id}"),
            };
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapPost("/departures", async (
            DepartureCreateRequest request,
            ICommandHandler<CreateDepartureFlightCommand, CreateFlightResult> handler,
            IValidator<DepartureCreateRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new CreateDepartureFlightCommand(request), ct);
            return result.Outcome switch
            {
                CreateFlightOutcome.DuplicateFlight => ApiResult.Conflict(ctx, "DUPLICATE_FLIGHT",
                    "A flight with this airline, flight number and date already exists."),
                _ => ApiResult.Created(ctx, result.Flight!, $"/api/flights/{result.Flight!.Id}"),
            };
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapPut("/arrivals/{id:long}", async (
            long id,
            UpdateArrivalFlightRequest request,
            ICommandHandler<UpdateArrivalFlightCommand, UpdateFlightResult> handler,
            IValidator<UpdateArrivalFlightRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateArrivalFlightCommand(id, request), ct);
            return result.Outcome switch
            {
                UpdateFlightOutcome.NotFound => ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Arrival flight {id} not found."),
                UpdateFlightOutcome.DuplicateFlight => ApiResult.Conflict(ctx, "DUPLICATE_FLIGHT",
                    "A flight with this airline, flight number and date already exists."),
                _ => ApiResult.Ok(ctx, result.Flight!),
            };
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapPut("/departures/{id:long}", async (
            long id,
            UpdateDepartureFlightRequest request,
            ICommandHandler<UpdateDepartureFlightCommand, UpdateFlightResult> handler,
            IValidator<UpdateDepartureFlightRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateDepartureFlightCommand(id, request), ct);
            return result.Outcome switch
            {
                UpdateFlightOutcome.NotFound => ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Departure flight {id} not found."),
                UpdateFlightOutcome.DuplicateFlight => ApiResult.Conflict(ctx, "DUPLICATE_FLIGHT",
                    "A flight with this airline, flight number and date already exists."),
                _ => ApiResult.Ok(ctx, result.Flight!),
            };
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapDelete("/arrivals/{id:long}", async (
            long id,
            bool? deleteLinkedFlight,
            ICommandHandler<DeleteArrivalFlightCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteArrivalFlightCommand(id, deleteLinkedFlight ?? false), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Arrival flight {id} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapDelete("/departures/{id:long}", async (
            long id,
            bool? deleteLinkedFlight,
            ICommandHandler<DeleteDepartureFlightCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteDepartureFlightCommand(id, deleteLinkedFlight ?? false), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Departure flight {id} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapPost("/arrivals/{id:long}/unlink", async (
            long id,
            ICommandHandler<UnlinkArrivalFlightCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var unlinked = await handler.HandleAsync(new UnlinkArrivalFlightCommand(id), ct);
            return unlinked
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "FLIGHT_NOT_LINKED", $"Arrival flight {id} not found or not linked to a departure.");
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapPost("/departures/{id:long}/unlink", async (
            long id,
            ICommandHandler<UnlinkDepartureFlightCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var unlinked = await handler.HandleAsync(new UnlinkDepartureFlightCommand(id), ct);
            return unlinked
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "FLIGHT_NOT_LINKED", $"Departure flight {id} not found or not linked to an arrival.");
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapPost("/arrivals/{id:long}/link", async (
            long id,
            LinkFlightRequest request,
            ICommandHandler<LinkFlightsCommand, LinkFlightsResult> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new LinkFlightsCommand(id, request.OtherFlightId), ct);
            return result switch
            {
                LinkFlightsResult.Success => ApiResult.NoContent(ctx),
                LinkFlightsResult.ArrivalNotFound => ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Arrival flight {id} not found."),
                LinkFlightsResult.DepartureNotFound => ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Departure flight {request.OtherFlightId} not found."),
                LinkFlightsResult.ArrivalAlreadyLinked => ApiResult.Conflict(ctx, "FLIGHT_ALREADY_LINKED", $"Arrival flight {id} is already linked. Unlink it first."),
                _ => ApiResult.Conflict(ctx, "FLIGHT_ALREADY_LINKED", $"Departure flight {request.OtherFlightId} is already linked. Unlink it first."),
            };
        }).RequireAuthorization(Permissions.FlightWrite);

        group.MapPost("/departures/{id:long}/link", async (
            long id,
            LinkFlightRequest request,
            ICommandHandler<LinkFlightsCommand, LinkFlightsResult> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new LinkFlightsCommand(request.OtherFlightId, id), ct);
            return result switch
            {
                LinkFlightsResult.Success => ApiResult.NoContent(ctx),
                LinkFlightsResult.ArrivalNotFound => ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Arrival flight {request.OtherFlightId} not found."),
                LinkFlightsResult.DepartureNotFound => ApiResult.NotFound(ctx, "FLIGHT_NOT_FOUND", $"Departure flight {id} not found."),
                LinkFlightsResult.ArrivalAlreadyLinked => ApiResult.Conflict(ctx, "FLIGHT_ALREADY_LINKED", $"Arrival flight {request.OtherFlightId} is already linked. Unlink it first."),
                _ => ApiResult.Conflict(ctx, "FLIGHT_ALREADY_LINKED", $"Departure flight {id} is already linked. Unlink it first."),
            };
        }).RequireAuthorization(Permissions.FlightWrite);

        return app;
    }
}
