using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Metadatas;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlight.Api.Endpoints.Metadatas;

public static class AirportImageEndpoints
{
    public static IEndpointRouteBuilder MapAirportImageEndpoints(this IEndpointRouteBuilder app)
    {
        const string basePath = "/api/metadata/airports/{code}/images";
        var group = app.MapGroup(basePath).WithTags("Metadata: Airport Images");

        group.MapGet("/", async (
            string code,
            IQueryHandler<ListAirportImagesQuery, IReadOnlyList<AirportImageResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListAirportImagesQuery(code), ct)))
            .RequireAuthorization(Permissions.MetadataView);

        group.MapGet("/{id:int}", async (
            string code,
            int id,
            IQueryHandler<GetAirportImageQuery, AirportImageData?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var image = await handler.HandleAsync(new GetAirportImageQuery(code, id), ct);
            return image is null
                ? ApiResult.NotFound(ctx, "AIRPORT_IMAGE_NOT_FOUND", $"Airport image {id} not found.")
                : Results.Bytes(image.Data, image.ContentType);
        }).RequireAuthorization(Permissions.MetadataView);

        group.MapPost("/", async (
            string code,
            IFormFile file,
            [FromForm] int? displayOrder,
            ICommandHandler<AddAirportImageCommand, AirportImageResponse> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validationError = ImageUploadValidation.Validate(file.Length, file.ContentType);
            if (validationError is not null)
                return ApiResult.BadRequest(ctx, "INVALID_IMAGE", validationError);

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream, ct);

            var result = await handler.HandleAsync(new AddAirportImageCommand(code, stream.ToArray(), file.ContentType, displayOrder), ct);
            return ApiResult.Created(ctx, result, $"{basePath.Replace("{code}", code)}/{result.Id}");
        }).RequireAuthorization(Permissions.MetadataCreate).DisableAntiforgery();

        group.MapDelete("/{id:int}", async (
            string code,
            int id,
            ICommandHandler<DeleteAirportImageCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteAirportImageCommand(code, id), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "AIRPORT_IMAGE_NOT_FOUND", $"Airport image {id} not found.");
        }).RequireAuthorization(Permissions.MetadataDelete);

        return app;
    }
}
