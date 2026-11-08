using Aero.Shared.Application;
using Aero.Shared.Application.Commands.RolePermissions;
using Aero.Shared.Application.Commands.Roles;
using Aero.Shared.Application.Queries.Audits;
using Aero.Shared.Application.Queries.RolePermissions;
using Aero.Shared.Application.Queries.Roles;
using Aero.Shared.Contracts.Requests;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Domain.Enums;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Domain.Identity.Audits;
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

        group.MapGet("/{name}", async (
            string name,
            IQueryHandler<GetRoleQuery, RoleResponse?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetRoleQuery(name), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "ROLE_NOT_FOUND", $"Role {name} not found.")
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
            return ApiResult.Created(ctx, result, $"/api/roles/{result.Name}");
        });

        group.MapPut("/{id:int}", async (
            string name,
            UpdateRoleCommand command,
            ICommandHandler<UpdateRoleCommand, RoleResponse?> handler,
            IValidator<UpdateRoleCommand> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(command with { Name = name }, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(command with { Name = name }, ct);
            return result is null
                ? ApiResult.NotFound(ctx, "ROLE_NOT_FOUND", $"Role {name} not found.")
                : ApiResult.Ok(ctx, result);
        });

        group.MapGet("/{roleName}/permissions", async (
            string roleName,
            IQueryHandler<GetRolePermissionsQuery, IReadOnlyList<string>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetRolePermissionsQuery(roleName), ct)))
            .RequireAuthorization(Permissions.RoleManage);

        group.MapPut("/{roleName}/permissions", async (
            string roleName,
            SetPermissionsRequest request,
            ICommandHandler<SetRolePermissionsCommand> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            await handler.HandleAsync(new SetRolePermissionsCommand(roleName, request.PermissionCodes), ct);
            return ApiResult.NoContent(ctx);
        }).RequireAuthorization(Permissions.RoleManage);

        group.MapDelete("/{name}", async (
            string name,
            ICommandHandler<DeleteRoleCommand, DeleteRoleResult> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new DeleteRoleCommand(name), ct);
            return result switch
            {
                DeleteRoleResult.NotFound => ApiResult.NotFound(ctx, "ROLE_NOT_FOUND", $"Role {name} not found."),
                DeleteRoleResult.Protected => ApiResult.Conflict(ctx, "ROLE_PROTECTED", "The default Admin role cannot be deleted."),
                _ => ApiResult.NoContent(ctx),
            };
        }).RequireAuthorization(Permissions.RoleManage);

        group.MapGet("/{id:int}/audit", async (
            int id,
            IQueryHandler<AuditQuery<Role>, IReadOnlyList<AuditEntryResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new AuditQuery<Role>(id.ToString()), ct)))
            .RequireAuthorization(Permissions.RoleManage);

        return app;
    }
}






