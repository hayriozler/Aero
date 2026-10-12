using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Infrastructure.Extensions;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Roles;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles")
            .RequireAuthorization(Policies.AirportOrRamp);

        group.MapGet("/", GetAll)
            .WithName("GetAllRoles")            
            .Produces<IReadOnlyList<RoleDto>>();

        group.MapGet("/{name}", GetByName)
            .WithName("GetRoleById")
            .Produces<RoleDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .RequireAuthorization(Policies.AirportOperator)
            .WithName("CreateRole")
            .AddEndpointFilter<ValidationFilter<CreateRoleRequest>>()
            .Produces<RoleDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(409);

        group.MapPut("/{name}", Update)
            .RequireAuthorization(Policies.AirportOperator)
            .WithName("UpdateRole")
            .AddEndpointFilter<ValidationFilter<UpdateRoleRequest>>()
            .Produces<RoleDto>()
            .ProducesProblem(403)
            .ProducesProblem(404);

        group.MapDelete("/{name}", Delete)
            .RequireAuthorization(Policies.AirportOperator)
            .WithName("DeleteRole")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<RoleDto>>> GetAll(
        AeroScanDataContext db, CancellationToken ct)
    {
        var roles = await db.RoleSet
            .AsNoTracking()
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<RoleDto>>([.. roles.Select(ToDto)]);
    }

    private static async Task<Results<Ok<RoleDto>, NotFound>> GetByName(
        string name, AeroScanDataContext db, CancellationToken ct)
    {
        var role = await db.RoleSet
            .AsNoTracking()
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Name == name, ct);

        return role is null ? TypedResults.NotFound() : TypedResults.Ok(ToDto(role));
    }

    private static async Task<Results<Created<RoleDto>, BadRequest<string>, ForbidHttpResult, Conflict<string>>> Create(
        CreateRoleRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var exists = await db.RoleSet.AnyAsync(r => r.Name == request.Name.ToLowerInvariant().Trim(), ct);
        if (exists) return TypedResults.Conflict($"Role '{request.Name}' already exists.");

        var role = Role.Create(request.Name, request.DisplayName);
        db.RoleSet.Add(role);
        await db.SaveChangesAsync(ct);

        // Assign permissions after role has an Id
        foreach (var permId in request.PermissionIds.Distinct())
        {
            if (await db.PermissionSet.AnyAsync(p => p.Id == permId, ct))
                role.AddPermission(permId);
        }
        await db.SaveChangesAsync(ct);

        await db.Entry(role).Collection(r => r.RolePermissions).LoadAsync(ct);
        foreach (var rp in role.RolePermissions)
            await db.Entry(rp).Reference(x => x.Permission).LoadAsync(ct);

        return TypedResults.Created($"/api/roles/{role.Name}", ToDto(role));
    }

    private static async Task<Results<Ok<RoleDto>, ForbidHttpResult, NotFound>> Update(
        string name, UpdateRoleRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var role = await db.RoleSet
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Name == name, ct);

        if (role is null) return TypedResults.NotFound();

        role.Update(request.DisplayName);

        // Replace permissions
        role.SetPermissions(request.PermissionIds
            .Where(pid => db.PermissionSet.Any(p => p.Id == pid))
            .Distinct());

        await db.SaveChangesAsync(ct);

        // Reload permissions navigation
        await db.Entry(role).Collection(r => r.RolePermissions).LoadAsync(ct);
        foreach (var rp in role.RolePermissions)
            await db.Entry(rp).Reference(x => x.Permission).LoadAsync(ct);

        return TypedResults.Ok(ToDto(role));
    }

    private static async Task<Results<NoContent, BadRequest<string>, ForbidHttpResult, NotFound>> Delete(
        string name, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var role = await db.RoleSet.Include(r => r.UserRoles).FirstOrDefaultAsync(r => r.Name == name, ct);
        if (role is null) return TypedResults.NotFound();

        if (role.UserRoles.Count > 0)
            return TypedResults.BadRequest($"Cannot delete role '{role.Name}' — it is assigned to {role.UserRoles.Count} user(s).");

        db.RoleSet.Remove(role);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static RoleDto ToDto(Role r) => new(
        r.Name,
        r.DisplayName,
        r.UserRoles.Count,
        [.. r.RolePermissions
            .Where(rp => rp.Permission is not null)
            .Select(rp => new PermissionDto(
                rp.Permission.Id,
                rp.Permission.Name,
                rp.Permission.DisplayName,
                rp.Permission.Description,
                rp.Permission.Group))]);
}
