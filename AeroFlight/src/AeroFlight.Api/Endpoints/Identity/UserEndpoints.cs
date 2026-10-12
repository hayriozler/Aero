using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Identity.Users;
using AeroFlight.Application.Metadatas;
using AeroFlight.Contracts.Identity;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Identity;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.Identity;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization();

        group.MapGet("/", async (
            IQueryHandler<GetUsersQuery, IReadOnlyList<UserResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetUsersQuery(), ct)))
            .RequireAuthorization(Permissions.UserManage);

        group.MapGet("/{id:int}", async (
            int id,
            IQueryHandler<GetUserQuery, UserResponse?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetUserQuery(id), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "USER_NOT_FOUND", $"User {id} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.UserManage);

        group.MapPost("/", async (
            CreateUserCommand command,
            ICommandHandler<CreateUserCommand, UserResponse> handler,
            IValidator<CreateUserCommand> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(command, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(command, ct);
            return ApiResult.Created(ctx, result, $"/api/users/{result.Id}");
        });

        group.MapPut("/{id:int}", async (
            int id,
            UpdateUserCommand command,
            ICommandHandler<UpdateUserCommand, UserResponse?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(command with { Id = id }, ct);
            return result is null
                ? ApiResult.NotFound(ctx, "USER_NOT_FOUND", $"User {id} not found.")
                : ApiResult.Ok(ctx, result);
        });

        group.MapPost("/{id:int}/roles", async (
            int id,
            AssignRoleRequest request,
            ICommandHandler<AssignRoleCommand> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            await handler.HandleAsync(new AssignRoleCommand(id, request.RoleId), ct);
            return ApiResult.NoContent(ctx);
        });

        group.MapDelete("/{id:int}/roles/{roleId:int}", async (
            int id,
            int roleId,
            ICommandHandler<RemoveRoleCommand> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            await handler.HandleAsync(new RemoveRoleCommand(id, roleId), ct);
            return ApiResult.NoContent(ctx);
        });

        group.MapDelete("/{id:int}", async (
            int id,
            ICommandHandler<DeleteUserCommand, DeleteUserResult> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new DeleteUserCommand(id), ct);
            return result switch
            {
                DeleteUserResult.NotFound => ApiResult.NotFound(ctx, "USER_NOT_FOUND", $"User {id} not found."),
                DeleteUserResult.Protected => ApiResult.Conflict(ctx, "USER_PROTECTED", "The default admin user cannot be deleted."),
                _ => ApiResult.NoContent(ctx),
            };
        }).RequireAuthorization(Permissions.UserManage);

        group.MapGet("/{id:int}/audit", async (
            int id,
            IQueryHandler<MetadataAuditQuery<User>, IReadOnlyList<AuditEntryResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new MetadataAuditQuery<User>(id.ToString()), ct)))
            .RequireAuthorization(Permissions.UserManage);

        return app;
    }
}

public record AssignRoleRequest(int RoleId);
