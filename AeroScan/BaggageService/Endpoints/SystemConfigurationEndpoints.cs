using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Settings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class SystemConfigurationEndpoints
{
    public static IEndpointRouteBuilder MapSystemConfigurationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/system-configs")
            .RequireAuthorization(Policies.AirportOperator);

        group.MapGet("/", GetAll)
            .WithName("GetAllSystemConfigurations")
            .Produces<IReadOnlyList<SystemConfigurationDto>>();

        group.MapGet("/{key}", GetByKey)
            .WithName("GetSystemConfigurationByKey")
            .Produces<SystemConfigurationDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .WithName("CreateSystemConfiguration")
            .AddEndpointFilter<ValidationFilter<CreateSystemConfigurationRequest>>()
            .Produces<SystemConfigurationDto>(201)
            .ProducesProblem(409);

        group.MapPut("/{key}", Update)
            .WithName("UpdateSystemConfiguration")
            .AddEndpointFilter<ValidationFilter<UpdateSystemConfigurationRequest>>()
            .Produces<SystemConfigurationDto>()
            .ProducesProblem(404);

        group.MapDelete("/{key}", Delete)
            .WithName("DeleteSystemConfiguration")
            .Produces(204)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<SystemConfigurationDto>>> GetAll(
        AeroScanDataContext db, CancellationToken ct)
    {
        var items = await db.SystemConfigurationSet
            .AsNoTracking()
            .OrderBy(s => s.Key)
            .Select(s => ToDto(s))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<SystemConfigurationDto>>(items);
    }

    private static async Task<Results<Ok<SystemConfigurationDto>, NotFound>> GetByKey(
        string key, AeroScanDataContext db, CancellationToken ct)
    {
        var item = await db.SystemConfigurationSet.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key.ToUpperInvariant(), ct);

        return item is null ? TypedResults.NotFound() : TypedResults.Ok(ToDto(item));
    }

    private static async Task<Results<Created<SystemConfigurationDto>, Conflict<string>>> Create(
        CreateSystemConfigurationRequest request, AeroScanDataContext db, HttpContext ctx, CancellationToken ct)
    {
        var exists = await db.SystemConfigurationSet.AnyAsync(s => s.Key == request.Key.ToUpperInvariant(), ct);
        if (exists)
            return TypedResults.Conflict($"System configuration '{request.Key}' already exists.");

        var username = ctx.User.FindFirst("unique_name")?.Value ?? "system";
        var item = SystemConfiguration.Create(request.Key, request.Value, request.Description);
        db.SystemConfigurationSet.Add(item);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/system-configurations/{item.Key}", ToDto(item));
    }

    private static async Task<Results<Ok<SystemConfigurationDto>, NotFound>> Update(
        string key, UpdateSystemConfigurationRequest request, AeroScanDataContext db, HttpContext ctx, CancellationToken ct)
    {
        var item = await db.SystemConfigurationSet
            .FirstOrDefaultAsync(s => s.Key == key.ToUpperInvariant(), ct);

        if (item is null) return TypedResults.NotFound();

        item.Update(request.Value, request.Description);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(item));
    }

    private static async Task<Results<NoContent, NotFound>> Delete(
        string key, AeroScanDataContext db, CancellationToken ct)
    {
        var item = await db.SystemConfigurationSet
            .FirstOrDefaultAsync(s => s.Key == key.ToUpperInvariant(), ct);

        if (item is null) return TypedResults.NotFound();

        db.SystemConfigurationSet.Remove(item);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static SystemConfigurationDto ToDto(SystemConfiguration s) => new(s.Key, s.Value, s.Description);
}