using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.CodeShares;
using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Identity;
using AeroFlight.Domain.Ops.Resources;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.Flights;

public static class FlightCodeShareEndpoints
{
    public static IEndpointRouteBuilder MapFlightCodeShareEndpoints(this IEndpointRouteBuilder app)
    {
        MapCodeShareEndpoints<OpsArrivalCodeShare>(app, "/api/flights/arrivals/{flightId:long}/codeshares");
        MapCodeShareEndpoints<OpsDepartureCodeShare>(app, "/api/flights/departures/{flightId:long}/codeshares");

        return app;
    }

    private static void MapCodeShareEndpoints<TCodeShare>(IEndpointRouteBuilder app, string basePath)
        where TCodeShare : OpsCodeShare
    {
        app.MapGet(basePath, async (
            long flightId,
            IQueryHandler<ListCodeSharesQuery<TCodeShare>, IReadOnlyList<CodeShareResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListCodeSharesQuery<TCodeShare>(flightId), ct)))
            .RequireAuthorization(Permissions.FlightView);

        app.MapPost(basePath, async (
            long flightId,
            CodeShareRequest request,
            ICommandHandler<AddCodeShareCommand<TCodeShare>, CodeShareResponse> handler,
            IValidator<CodeShareRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new AddCodeShareCommand<TCodeShare>(flightId, request), ct);
            return ApiResult.Created(ctx, result, $"{basePath}/{result.Id}");
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapPut($"{basePath}/{{codeShareId:long}}", async (
            long flightId,
            long codeShareId,
            CodeShareRequest request,
            ICommandHandler<UpdateCodeShareCommand<TCodeShare>, CodeShareResponse?> handler,
            IValidator<CodeShareRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateCodeShareCommand<TCodeShare>(flightId, codeShareId, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "CODESHARE_NOT_FOUND", $"Codeshare {codeShareId} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.FlightWrite);

        app.MapDelete($"{basePath}/{{codeShareId:long}}", async (
            long flightId,
            long codeShareId,
            ICommandHandler<DeleteCodeShareCommand<TCodeShare>, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteCodeShareCommand<TCodeShare>(flightId, codeShareId), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "CODESHARE_NOT_FOUND", $"Codeshare {codeShareId} not found.");
        }).RequireAuthorization(Permissions.FlightWrite);
    }
}
