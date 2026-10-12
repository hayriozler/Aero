using BaggageService.Filters;
using BaggageService.Persistence;
using BaggageService.Services;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Users;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BaggageService.Endpoints;

public static class AuthEndpoints
{
    private const int _refreshTokenDays = 7;

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth");

        auth.MapPost("/login", Login)
            .WithName("Login")
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .Produces<LoginResponseDto>()
            .ProducesProblem(401);

        auth.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<RefreshRequest>>()
            .Produces<LoginResponseDto>()
            .ProducesProblem(401);

        auth.MapPost("/change-password", ChangePassword)
            .WithName("ChangePassword")
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<ChangePasswordRequest>>()
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

        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
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

        if (!PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return TypedResults.BadRequest("Current password is incorrect.");

        var result = user.UpdatePassword(PasswordHasher.Hash(request.NewPassword));
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
        string token, string refreshToken, DateTime expiresAt, User user) =>
        new(
            Token:        token,
            RefreshToken: refreshToken,
            Username:     user.Username,
            DisplayName:  user.DisplayName,
            Roles:        [.. user.GetRoleNames()],
            CompanyCode:  user.CompanyCode,
            CompanyName:  user.Company.Name,
            CompanyType:  user.Company.Type.ToString(),
            ExpiresAt:    expiresAt);
}
