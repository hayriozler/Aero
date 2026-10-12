using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Infrastructure.Extensions;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Users;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .RequireAuthorization(Policies.AirportOrRamp);

        group.MapGet("/", GetAll)
            .WithName("GetAllUsers")
            .Produces<IReadOnlyList<UserDto>>();

        group.MapGet("/{id:int}", GetById)
            .WithName("GetUserById")
            .Produces<UserDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .WithName("CreateUser")
            .AddEndpointFilter<ValidationFilter<CreateUserRequest>>()
            .Produces<UserDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(409);

        group.MapPut("/{id:int}", Update)
            .WithName("UpdateUser")
            .AddEndpointFilter<ValidationFilter<UpdateUserRequest>>()
            .Produces<UserDto>()
            .ProducesProblem(403)
            .ProducesProblem(404);

        group.MapDelete("/{id:int}", Deactivate)
            .WithName("DeactivateUser")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(404);

        group.MapPost("/{id:int}/reset-password", ResetPassword)
            .WithName("AdminResetPassword")
            .AddEndpointFilter<ValidationFilter<AdminResetPasswordRequest>>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(404);

        return app;
    }

    // ── GET /api/users ────────────────────────────────────────────────────────
    private static async Task<Ok<IReadOnlyList<UserDto>>> GetAll(
        HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (callerCompanyCode, isAirport) = ctx.GetCompanyContext();

        var query = db.UserSet
            .AsNoTracking()
            .Include(u => u.Company)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!isAirport)
            query = query.Where(u => u.CompanyCode == callerCompanyCode);

        var users = await query
            .OrderBy(u => u.Username)
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<UserDto>>([.. users.Select(ToDto)]);
    }

    // ── GET /api/users/{id} ───────────────────────────────────────────────────
    private static async Task<Results<Ok<UserDto>, NotFound>> GetById(
        int id, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (callerCompanyCode, isAirport) = ctx.GetCompanyContext();

        var user = await db.UserSet
            .AsNoTracking()
            .Include(u => u.Company)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null) return TypedResults.NotFound();
        if (!isAirport && user.CompanyCode != callerCompanyCode) return TypedResults.NotFound();

        return TypedResults.Ok(ToDto(user));
    }

    // ── POST /api/users ───────────────────────────────────────────────────────
    private static async Task<Results<Created<UserDto>, BadRequest<string>, Conflict<string>>> Create(
        CreateUserRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (callerCompanyCode, isAirport) = ctx.GetCompanyContext();

        // Handling agents can only create users in their own company
        var targetCompanyCode = isAirport ? request.CompanyCode : callerCompanyCode;

        if (!isAirport && request.CompanyCode != callerCompanyCode)
            return TypedResults.BadRequest("You can only create users for your own company.");

        var companyExists = await db.CompanySet.AnyAsync(c => c.Code == targetCompanyCode, ct);
        if (!companyExists) return TypedResults.BadRequest("Company not found.");

        var usernameExists = await db.UserSet
            .AnyAsync(u => u.Username == request.Username.ToLowerInvariant().Trim(), ct);
        if (usernameExists)
            return TypedResults.Conflict($"Username '{request.Username}' is already taken.");

        var passwordHash = PasswordHasher.Hash(request.Password);
        var user = User.Create(request.Username, request.DisplayName, passwordHash, targetCompanyCode);
        db.UserSet.Add(user);
        await db.SaveChangesAsync(ct);

        foreach (var roleName in request.RoleNames.Distinct())
        {
            if (await db.RoleSet.AnyAsync(r => r.Name == roleName, ct))
                user.AssignRole(roleName);
        }
        await db.SaveChangesAsync(ct);

        var created = await db.UserSet
            .Include(u => u.Company)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstAsync(u => u.Id == user.Id, ct);

        return TypedResults.Created($"/api/users/{user.Id}", ToDto(created));
    }

    // ── PUT /api/users/{id} ───────────────────────────────────────────────────
    // Airport operators can edit any user.
    // Handling agents can only edit users within their own company.
    private static async Task<Results<Ok<UserDto>, ForbidHttpResult, NotFound>> Update(
        int id, UpdateUserRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (callerCompanyCode, isAirport) = ctx.GetCompanyContext();

        var user = await db.UserSet
            .Include(u => u.Company)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null) return TypedResults.NotFound();

        // Handling agent: not own company → 404
        if (!isAirport && user.CompanyCode != callerCompanyCode)
            return TypedResults.NotFound();

        user.UpdateDetails(request.DisplayName);

        // Replace roles
        foreach (var ur in user.UserRoles.ToList())
            user.RemoveRole(ur.RoleName);
        foreach (var roleName in request.RoleNames.Distinct())
        {
            if (await db.RoleSet.AnyAsync(r => r.Name == roleName, ct))
                user.AssignRole(roleName);
        }
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(user));
    }

    // ── DELETE /api/users/{id} (soft-delete / deactivate) ────────────────────
    private static async Task<Results<NoContent, BadRequest<string>, ForbidHttpResult, NotFound>> Deactivate(
        int id, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (callerCompanyCode, isAirport) = ctx.GetCompanyContext();

        var user = await db.UserSet
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null) return TypedResults.NotFound();
        if (!isAirport && user.CompanyCode != callerCompanyCode) return TypedResults.NotFound();

        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    // ── POST /api/users/{id}/reset-password ───────────────────────────────────
    private static async Task<Results<NoContent, BadRequest<string>, ForbidHttpResult, NotFound>> ResetPassword(
        int id, AdminResetPasswordRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (callerCompanyCode, isAirport) = ctx.GetCompanyContext();

        var user = await db.UserSet.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return TypedResults.NotFound();
        if (!isAirport && user.CompanyCode != callerCompanyCode) return TypedResults.NotFound();

        var hash = PasswordHasher.Hash(request.NewPassword);
        user.UpdatePassword(hash);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }


    private static UserDto ToDto(User u) => new(
        u.Id,
        u.Username,
        u.DisplayName,
        [.. u.UserRoles.Where(ur => ur.Role is not null).Select(ur => ur.Role.Name)],
        u.Company?.Code ?? string.Empty,
        u.Company?.Name ?? string.Empty,
        u.Company?.Type.ToString() ?? string.Empty,
        u.LastLoginAt);
}
