using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Mappings;
using Domain.Common;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class AirlineClassMapEndpoints
{
    public static IEndpointRouteBuilder MapAirlineClassMapEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/airline-class-maps")
            .RequireAuthorization(Policies.AirportOrRamp);

        group.MapGet("/", GetAll)
            .WithName("GetAirlineClassMaps")
            .Produces<IReadOnlyList<AirlineClassMapDto>>();

        group.MapPost("/", Create)
            .WithName("CreateAirlineClassMap")
            .AddEndpointFilter<ValidationFilter<CreateAirlineClassMapRequest>>()
            .Produces<AirlineClassMapDto>(201)
            .ProducesProblem(409);

        group.MapPut("/{airlineCode}/{sourceClass}", Update)
            .WithName("UpdateAirlineClassMap")
            .AddEndpointFilter<ValidationFilter<UpdateAirlineClassMapRequest>>()
            .Produces<AirlineClassMapDto>()
            .ProducesProblem(404);

        group.MapDelete("/{airlineCode}/{sourceClass}", Delete)
            .WithName("DeleteAirlineClassMap")
            .Produces(204)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<AirlineClassMapDto>>> GetAll(
        AeroScanDataContext db, CancellationToken ct)
    {
        var items = await db.AirlineClassMapSet
            .AsNoTracking()
            .OrderBy(m => m.AirlineCode).ThenBy(m => m.SourceClass)
            .Select(m => ToDto(m))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<AirlineClassMapDto>>(items);
    }

    private static async Task<Results<Created<AirlineClassMapDto>, Conflict<string>>> Create(
        CreateAirlineClassMapRequest request, AeroScanDataContext db, HttpContext ctx, CancellationToken ct)
    {
        var exists = await db.AirlineClassMapSet.AnyAsync(
            m => m.AirlineCode == request.AirlineCode.ToUpperInvariant().Trim()
              && m.SourceClass  == request.SourceClass, ct);

        if (exists)
            return TypedResults.Conflict("A mapping for this airline code and source class already exists.");

        var username = ctx.User.FindFirst("unique_name")?.Value ?? "system";
        var map = AirlineClassMap.Create(request.AirlineCode, request.SourceClass, request.TargetClass);

        db.AirlineClassMapSet.Add(map);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/airline-class-maps/{map.AirlineCode}/{map.SourceClass}", ToDto(map));
    }

    private static async Task<Results<Ok<AirlineClassMapDto>, NotFound>> Update(
        string airlineCode, char sourceClass, UpdateAirlineClassMapRequest request,
        AeroScanDataContext db, HttpContext ctx, CancellationToken ct)
    {
        var map = await db.AirlineClassMapSet.FirstOrDefaultAsync(
            m => m.AirlineCode == airlineCode.ToUpperInvariant()
              && m.SourceClass  == sourceClass, ct);

        if (map is null) return TypedResults.NotFound();

        var username = ctx.User.FindFirst("unique_name")?.Value ?? "system";
        map.Update(request.TargetClass);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(map));
    }

    private static async Task<Results<NoContent, NotFound>> Delete(
        string airlineCode, char sourceClass, AeroScanDataContext db, CancellationToken ct)
    {
        var map = await db.AirlineClassMapSet.FirstOrDefaultAsync(
            m => m.AirlineCode == airlineCode.ToUpperInvariant()
              && m.SourceClass  == sourceClass, ct);

        if (map is null) return TypedResults.NotFound();

        db.AirlineClassMapSet.Remove(map);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static AirlineClassMapDto ToDto(AirlineClassMap m) => new(m.AirlineCode, m.SourceClass, m.TargetClass);

}
