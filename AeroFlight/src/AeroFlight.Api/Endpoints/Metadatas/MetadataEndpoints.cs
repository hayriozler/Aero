using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Metadatas;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Identity;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.Metadatas;

public static class MetadataEndpoints
{
    public static IEndpointRouteBuilder MapMetadataCrudEndpoints<TEntity, TRequest, TResponse>(
        this IEndpointRouteBuilder app, string route, string tag, Func<TRequest, string> getRequestCode)
        where TEntity : class
    {
        var group = app.MapGroup(route).WithTags(tag);
        MapReadEndpoints<TEntity, TResponse>(group, tag);

        group.MapGet("/{id}/audit", async (
            string id,
            IQueryHandler<MetadataAuditQuery<TEntity>, IReadOnlyList<AuditEntryResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new MetadataAuditQuery<TEntity>(id), ct)))
            .RequireAuthorization(Permissions.MetadataView);

        group.MapPost("/", async (
            TRequest request,
            ICommandHandler<MetadataCreateCommand<TEntity, TRequest>, TResponse> handler,
            IValidator<TRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new MetadataCreateCommand<TEntity, TRequest>(request), ct);
            return ApiResult.Created(ctx, result, $"{route}/{getRequestCode(request)}");
        }).RequireAuthorization(Permissions.MetadataCreate);

        group.MapPut("/{id}", async (
            string id,
            TRequest request,
            ICommandHandler<MetadataUpdateCommand<TEntity, TRequest>, TResponse?> handler,
            IValidator<TRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new MetadataUpdateCommand<TEntity, TRequest>(id, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "Metadata_NOT_FOUND", $"{tag} '{id}' not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.MetadataUpdate);

        group.MapDelete("/{id}", async (
            string id,
            ICommandHandler<MetadataDeleteCommand<TEntity>, MetadataDeleteResult> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new MetadataDeleteCommand<TEntity>(id), ct);
            return result.Outcome switch
            {
                MetadataDeleteOutcome.Deleted => ApiResult.NoContent(ctx),
                MetadataDeleteOutcome.InUse => ApiResult.Conflict(ctx, "Metadata_IN_USE", result.Reason ?? $"{tag} '{id}' is in use and cannot be deleted."),
                _ => ApiResult.NotFound(ctx, "Metadata_NOT_FOUND", $"{tag} '{id}' not found.")
            };
        }).RequireAuthorization(Permissions.MetadataDelete);

        return app;
    }

    public static IEndpointRouteBuilder MapMetadataLookupEndpoints<TEntity, TResponse>(
        this IEndpointRouteBuilder app, string route, string tag)
        where TEntity : class
    {
        var group = app.MapGroup(route).WithTags(tag);
        MapReadEndpoints<TEntity, TResponse>(group, tag);
        return app;
    }

    private static void MapReadEndpoints<TEntity, TResponse>(RouteGroupBuilder group, string tag)
        where TEntity : class
    {
        group.MapGet("/", async (
            IQueryHandler<MetadataListQuery<TEntity>, IReadOnlyList<TResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new MetadataListQuery<TEntity>(), ct)))
            .RequireAuthorization(Permissions.MetadataView);

        group.MapGet("/{id}", async (
            string id,
            IQueryHandler<MetadataGetQuery<TEntity>, TResponse?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new MetadataGetQuery<TEntity>(id), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "Metadata_NOT_FOUND", $"{tag} '{id}' not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.MetadataView);
    }
}
