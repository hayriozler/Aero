using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Application.Common;
using Aero.Flights.Application.Metadatas;
using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Identity;
using FluentValidation;

namespace Aero.Flights.Api.Endpoints.Metadatas;

public static class AirlineContactEndpoints
{
    public static IEndpointRouteBuilder MapAirlineContactEndpoints(this IEndpointRouteBuilder app)
    {
        const string basePath = "/api/metadata/airlines/{code}/contacts";
        var group = app.MapGroup(basePath).WithTags("Metadata: Airline Contacts");

        group.MapGet("/", async (
            string code,
            IQueryHandler<ListAirlineContactsQuery, IReadOnlyList<AirlineContactResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListAirlineContactsQuery(code), ct)))
            .RequireAuthorization(Permissions.MetadataView);

        group.MapPost("/", async (
            string code,
            AirlineContactRequest request,
            ICommandHandler<AddAirlineContactCommand, AirlineContactResponse> handler,
            IValidator<AirlineContactRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new AddAirlineContactCommand(code, request), ct);
            return ApiResult.Created(ctx, result, $"{basePath.Replace("{code}", code)}/{result.Id}");
        }).RequireAuthorization(Permissions.MetadataCreate);

        group.MapPut("/{id:int}", async (
            string code,
            int id,
            AirlineContactRequest request,
            ICommandHandler<UpdateAirlineContactCommand, AirlineContactResponse?> handler,
            IValidator<AirlineContactRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateAirlineContactCommand(code, id, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "AIRLINE_CONTACT_NOT_FOUND", $"Airline contact {id} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.MetadataUpdate);

        group.MapDelete("/{id:int}", async (
            string code,
            int id,
            ICommandHandler<DeleteAirlineContactCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteAirlineContactCommand(code, id), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "AIRLINE_CONTACT_NOT_FOUND", $"Airline contact {id} not found.");
        }).RequireAuthorization(Permissions.MetadataDelete);

        return app;
    }
}
