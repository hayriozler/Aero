using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Infrastructure.Extensions;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Permissions;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class PermissionEndpoints
{
    public static IEndpointRouteBuilder MapPermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/permissions")
            .RequireAuthorization(Policies.AirportOperator);

        group.MapGet("/", GetAll)
            .WithName("GetAllPermissions")
            .Produces<IReadOnlyList<PermissionDto>>();

        group.MapGet("/{id:int}", GetById)
            .WithName("GetPermissionById")
            .Produces<PermissionDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .WithName("CreatePermission")
            .AddEndpointFilter<ValidationFilter<CreatePermissionRequest>>()
            .Produces<PermissionDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(409);

        group.MapPut("/{id:int}", Update)
            .WithName("UpdatePermission")
            .AddEndpointFilter<ValidationFilter<UpdatePermissionRequest>>()
            .Produces<PermissionDto>()
            .ProducesProblem(403)
            .ProducesProblem(404);

        group.MapDelete("/{id:int}", Delete)
            .WithName("DeletePermission")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<PermissionDto>>> GetAll(
        AeroScanDataContext db, CancellationToken ct)
    {
        var permissions = await db.PermissionSet
            .AsNoTracking()
            .OrderBy(p => p.Group)
            .ThenBy(p => p.Name)
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<PermissionDto>>([.. permissions.Select(ToDto)]);
    }

    private static async Task<Results<Ok<PermissionDto>, NotFound>> GetById(
        int id, AeroScanDataContext db, CancellationToken ct)
    {
        var permission = await db.PermissionSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return permission is null ? TypedResults.NotFound() : TypedResults.Ok(ToDto(permission));
    }

    private static async Task<Results<Created<PermissionDto>, BadRequest<string>, ForbidHttpResult, Conflict<string>>> Create(
        CreatePermissionRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var exists = await db.PermissionSet.AnyAsync(
            p => p.Name == request.Name.ToLowerInvariant().Trim(), ct);
        if (exists) return TypedResults.Conflict($"Permission '{request.Name}' already exists.");

        var permission = Permission.Create(request.Name, request.DisplayName, request.Group, request.Description);
        db.PermissionSet.Add(permission);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/permissions/{permission.Id}", ToDto(permission));
    }

    private static async Task<Results<Ok<PermissionDto>, ForbidHttpResult, NotFound>> Update(
        int id, UpdatePermissionRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var permission = await db.PermissionSet.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (permission is null) return TypedResults.NotFound();

        permission.Update(request.DisplayName, request.Group, request.Description);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(permission));
    }

    private static async Task<Results<NoContent, BadRequest<string>, ForbidHttpResult, NotFound>> Delete(
        int id, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var permission = await db.PermissionSet
            .Include(p => p.RolePermissions)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        if (permission is null) return TypedResults.NotFound();

        if (permission.RolePermissions.Count > 0)
            return TypedResults.BadRequest(
                $"Cannot delete permission '{permission.Name}' — it is assigned to {permission.RolePermissions.Count} role(s).");

        db.PermissionSet.Remove(permission);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }


    private static PermissionDto ToDto(Permission p) =>
        new(p.Id, p.Name, p.DisplayName, p.Description, p.Group);
}
