using Aero.Shared.Application;
using Aero.Shared.Application.Commands.Roles;
using Aero.Shared.Contracts.Requests;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Domain.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aero.Shared.WebUtilities.EndPoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/permissions", (HttpContext ctx) =>
            ApiResult.Ok(ctx, Permissions.All.Select(p => new { p.Code, p.Description }).ToList()))
            .WithTags("Roles").RequireAuthorization(Permissions.RoleManage);

        var group = app.MapGroup("/api/roles").WithTags("Roles").RequireAuthorization();

        group.MapGet("/", async (
            IQueryHandler<GetRolesQuery, IReadOnlyList<RoleResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetRolesQuery(), ct)))
            .RequireAuthorization(Permissions.RoleManage);

        group.MapGet("/{id:int}", async (
            int id,
            IQueryHandler<GetRoleQuery, RoleResponse?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetRoleQuery(id), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "ROLE_NOT_FOUND", $"Role {id} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.RoleManage);

        group.MapPost("/", async (
            CreateRoleCommand command,
            ICommandHandler<CreateRoleCommand, RoleResponse> handler,
            IValidator<CreateRoleCommand> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(command, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(command, ct);
            return ApiResult.Created(ctx, result, $"/api/roles/{result.Id}");
        });

        group.MapPut("/{id:int}", async (
            int id,
            UpdateRoleCommand command,
            ICommandHandler<UpdateRoleCommand, RoleResponse?> handler,
            IValidator<UpdateRoleCommand> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(command with { Id = id }, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(command with { Id = id }, ct);
            return result is null
                ? ApiResult.NotFound(ctx, "ROLE_NOT_FOUND", $"Role {id} not found.")
                : ApiResult.Ok(ctx, result);
        });

        group.MapGet("/{id:int}/permissions", async (
            int id,
            IQueryHandler<GetRolePermissionsQuery, IReadOnlyList<string>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetRolePermissionsQuery(id), ct)))
            .RequireAuthorization(Permissions.RoleManage);

        group.MapPut("/{id:int}/permissions", async (
            int id,
            SetPermissionsRequest request,
            ICommandHandler<SetRolePermissionsCommand> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            await handler.HandleAsync(new SetRolePermissionsCommand(id, request.PermissionCodes), ct);
            return ApiResult.NoContent(ctx);
        }).RequireAuthorization(Permissions.RoleManage);

        group.MapDelete("/{id:int}", async (
            int id,
            ICommandHandler<DeleteRoleCommand, DeleteRoleResult> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new DeleteRoleCommand(id), ct);
            return result switch
            {
                DeleteRoleResult.NotFound => ApiResult.NotFound(ctx, "ROLE_NOT_FOUND", $"Role {id} not found."),
                DeleteRoleResult.Protected => ApiResult.Conflict(ctx, "ROLE_PROTECTED", "The default Admin role cannot be deleted."),
                _ => ApiResult.NoContent(ctx),
            };
        }).RequireAuthorization(Permissions.RoleManage);

        group.MapGet("/{id:int}/audit", async (
            int id,
            IQueryHandler<MetadataAuditQuery<Role>, IReadOnlyList<AuditEntryResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new MetadataAuditQuery<Role>(id.ToString()), ct)))
            .RequireAuthorization(Permissions.RoleManage);

        return app;
    }
}






