using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Services;
using Aero.Shared.WebUtilities.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Aero.Shared.WebUtilities.EndPoints;

public static class IdentityEndpoints
{
    private const int _refreshTokenDays = 7;

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth");

        auth.MapPost("/login", Login)
            .WithName("Login")
            .AllowAnonymous()
            .AddEndpointFilter<EndPointValidationFilter<LoginRequest>>()
            .Produces<LoginResponseDto>()
            .ProducesProblem(401);

        auth.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .AllowAnonymous()
            .AddEndpointFilter<EndPointValidationFilter<RefreshRequest>>()
            .Produces<LoginResponseDto>()
            .ProducesProblem(401);

        auth.MapPost("/change-password", ChangePassword)
            .WithName("ChangePassword")
            .RequireAuthorization()
            .AddEndpointFilter<EndPointValidationFilter<ChangePasswordRequest>>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401);

        auth.MapPost("/logout", Logout)
            .WithName("Logout")
            .AllowAnonymous()
            .Produces(204);

        return app;
    }

    private static async Task<Results<Ok<LoginResponseDto>, UnauthorizedHttpResult>> Login(
        LoginRequest request,
        AeroScanDataContext db,
        JwtTokenService tokenService,
        CancellationToken ct)
    {
        var user = await db.UserSet
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Company)
            .FirstOrDefaultAsync(
                u => u.Username == request.Username.ToLowerInvariant().Trim(), ct);

        if (user is null || !PasswordHasherService.Verify(request.Password, user.PasswordHash))
            return TypedResults.Unauthorized();

        user.RecordLogin();

        // Revoke all existing active refresh tokens for this user
        await db.RefreshTokenSet
            .Where(t => t.UserId == user.Id && t.RevokedAt == null)
            .ExecuteUpdateAsync(t => t.SetProperty(x => x.RevokedAt, DateTime.UtcNow), ct);

        var (token, expiresAt) = tokenService.GenerateToken(user);

        var rawRefreshToken = JwtTokenService.GenerateRefreshToken();
        db.RefreshTokenSet.Add(RefreshToken.Create(
            user.Id,
            JwtTokenService.HashRefreshToken(rawRefreshToken),
            DateTime.UtcNow.AddDays(_refreshTokenDays)));

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(BuildResponse(token, rawRefreshToken, expiresAt, user));
    }

    private static async Task<Results<Ok<LoginResponseDto>, UnauthorizedHttpResult>> Refresh(
        RefreshRequest request,
        AeroScanDataContext db,
        JwtTokenService tokenService,
        CancellationToken ct)
    {
        var tokenHash = JwtTokenService.HashRefreshToken(request.RefreshToken);

        var existing = await db.RefreshTokenSet
            .Include(t => t.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .Include(t => t.User)
                .ThenInclude(u => u.Company)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

        if (existing is null || !existing.IsActive)
            return TypedResults.Unauthorized();

        existing.Revoke();

        var (newToken, expiresAt) = tokenService.GenerateToken(existing.User);

        var rawRefreshToken = JwtTokenService.GenerateRefreshToken();
        db.RefreshTokenSet.Add(RefreshToken.Create(
            existing.UserId,
            JwtTokenService.HashRefreshToken(rawRefreshToken),
            DateTime.UtcNow.AddDays(_refreshTokenDays)));

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(BuildResponse(newToken, rawRefreshToken, expiresAt, existing.User));
    }

    private static async Task<Results<NoContent, BadRequest<string>, UnauthorizedHttpResult>> ChangePassword(
        ChangePasswordRequest request,
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var subClaim = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)
                    ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (subClaim is null || !int.TryParse(subClaim.Value, out var userId))
            return TypedResults.Unauthorized();

        var user = await db.UserSet.FindAsync([userId], ct);
        if (user is null) return TypedResults.Unauthorized();

        if (!PasswordHasherService.Verify(request.CurrentPassword, user.PasswordHash))
            return TypedResults.BadRequest("Current password is incorrect.");

        var result = user.UpdatePassword(PasswordHasherService.Hash(request.NewPassword));
        if (result.IsFailure) return TypedResults.BadRequest(result.Error!);

        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> Logout(
        LogoutRequest? request,
        AeroScanDataContext db,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request?.RefreshToken))
        {
            var tokenHash = JwtTokenService.HashRefreshToken(request.RefreshToken);
            await db.RefreshTokenSet
                .Where(t => t.TokenHash == tokenHash && t.RevokedAt == null)
                .ExecuteUpdateAsync(t => t.SetProperty(x => x.RevokedAt, DateTime.UtcNow), ct);
        }
        return TypedResults.NoContent();
    }

    private static LoginResponseDto BuildResponse(
        string token, string refreshToken, DateTime expiresAt, ScannerUser user) =>
        new(
            Token: token,
            RefreshToken: refreshToken,
            Username: user.Username,
            DisplayName: user.DisplayName,
            Roles: [.. user.GetRoleNames()],
            CompanyCode: user.CompanyCode,
            CompanyName: user.Company.Name,
            CompanyType: user.Company.Type.ToString(),
            ExpiresAt: expiresAt);
}

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






