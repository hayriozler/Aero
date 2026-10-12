using System.Security.Claims;
using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Identity;
using AeroFlight.Application.Identity.Auth;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.Identity;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (
            LoginCommand command,
            ICommandHandler<LoginCommand, LoginResponse?> handler,
            IValidator<LoginCommand> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(command, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(command, ct);
            return result is null
                ? ApiResult.Unauthorized(ctx)
                : ApiResult.Ok(ctx, result);
        });

        // Client-credentials login for a service principal (see ClientApp) — e.g. AeroFids warming
        // its board — instead of impersonating a human user.
        group.MapPost("/client-token", async (
            ClientTokenCommand command,
            ICommandHandler<ClientTokenCommand, ClientTokenResponse?> handler,
            IValidator<ClientTokenCommand> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(command, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(command, ct);
            return result is null
                ? ApiResult.Unauthorized(ctx)
                : ApiResult.Ok(ctx, result);
        });

        // Lets a caller check what the just-issued token is allowed to do, without duplicating
        // the role→permission resolution that PermissionAuthorizationHandler does per-endpoint.
        group.MapGet("/permissions", async (HttpContext ctx, IUserPermissionService permissions, CancellationToken ct) =>
        {
            var roles = ctx.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var granted = await permissions.GetPermissionsAsync(roles, ct);
            return ApiResult.Ok(ctx, granted);
        }).RequireAuthorization();

        return app;
    }
}
