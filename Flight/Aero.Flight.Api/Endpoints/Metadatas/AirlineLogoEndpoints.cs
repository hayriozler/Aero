using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Application.Common;
using Aero.Flights.Application.Metadatas;
using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Aero.Flights.Api.Endpoints.Metadatas;

public static class AirlineLogoEndpoints
{
    public static IEndpointRouteBuilder MapAirlineLogoEndpoints(this IEndpointRouteBuilder app)
    {
        const string basePath = "/api/metadata/airlines/{code}/logos";
        var group = app.MapGroup(basePath).WithTags("Metadata: Airline Logos");

        group.MapGet("/", async (
            string code,
            IQueryHandler<ListAirlineLogosQuery, IReadOnlyList<AirlineLogoResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListAirlineLogosQuery(code), ct)))
            .RequireAuthorization(Permissions.MetadataView);

        group.MapGet("/{id:int}", async (
            string code,
            int id,
            IQueryHandler<GetAirlineLogoImageQuery, AirlineLogoImage?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var image = await handler.HandleAsync(new GetAirlineLogoImageQuery(code, id), ct);
            return image is null
                ? ApiResult.NotFound(ctx, "AIRLINE_LOGO_NOT_FOUND", $"Logo {id} not found for airline '{code}'.")
                : Results.Bytes(image.Data, image.ContentType);
        }).RequireAuthorization(Permissions.MetadataView);

        group.MapPost("/", async (
            string code,
            IFormFile file,
            [FromForm] string name,
            ICommandHandler<AddAirlineLogoCommand, AirlineLogoResponse> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validationError = ImageUploadValidation.Validate(file.Length, file.ContentType);
            if (validationError is not null)
                return ApiResult.BadRequest(ctx, "INVALID_IMAGE", validationError);
            if (string.IsNullOrWhiteSpace(name))
                return ApiResult.BadRequest(ctx, "INVALID_LOGO_NAME", "Name is required.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream, ct);

            var result = await handler.HandleAsync(new AddAirlineLogoCommand(code, name.Trim(), stream.ToArray(), file.ContentType), ct);
            return ApiResult.Created(ctx, result, $"{basePath.Replace("{code}", code)}/{result.Id}");
        }).RequireAuthorization(Permissions.MetadataCreate).DisableAntiforgery();

        group.MapPut("/{id:int}", async (
            string code,
            int id,
            AirlineLogoRenameRequest request,
            ICommandHandler<UpdateAirlineLogoNameCommand, AirlineLogoResponse?> handler,
            IValidator<AirlineLogoRenameRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateAirlineLogoNameCommand(code, id, request.Name.Trim()), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "AIRLINE_LOGO_NOT_FOUND", $"Logo {id} not found for airline '{code}'.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.MetadataUpdate);

        group.MapDelete("/{id:int}", async (
            string code,
            int id,
            ICommandHandler<DeleteAirlineLogoCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteAirlineLogoCommand(code, id), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "AIRLINE_LOGO_NOT_FOUND", $"Logo {id} not found for airline '{code}'.");
        }).RequireAuthorization(Permissions.MetadataDelete);

        return app;
    }
}
