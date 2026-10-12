using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Infrastructure.Extensions;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.HHTs;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class HandheldTerminalEndpoints
{
    public static IEndpointRouteBuilder MapHandheldTerminalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hht")
            .RequireAuthorization(Policies.AirportOrRamp);

        group.MapGet("/", GetAll)
            .WithName("GetAllHht")
            .Produces<IReadOnlyList<HandheldTerminalDto>>();

        group.MapGet("/{id:int}", GetById)
            .WithName("GetHhtById")
            .Produces<HandheldTerminalDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .RequireAuthorization([Policies.AirportOperator])
            .WithName("CreateHht")
            .AddEndpointFilter<ValidationFilter<CreateHandheldTerminalRequest>>()
            .Produces<HandheldTerminalDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(409);

        group.MapPut("/{id:int}", Update)
            .RequireAuthorization([Policies.AirportOperator])
            .WithName("UpdateHht")
            .AddEndpointFilter<ValidationFilter<UpdateHandheldTerminalRequest>>()
            .Produces<HandheldTerminalDto>()
            .ProducesProblem(403)
            .ProducesProblem(404);

        group.MapPost("/{id:int}/assign", Assign)
            .WithName("AssignHht")
            .AddEndpointFilter<ValidationFilter<AssignHandheldTerminalRequest>>()
            .Produces<HandheldTerminalDto>()
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(404);

        group.MapDelete("/{id:int}/assign", Unassign)
            .WithName("UnassignHht")
            .Produces<HandheldTerminalDto>()
            .ProducesProblem(403)
            .ProducesProblem(404);        

        return app;
    }

    private static async Task<Ok<IReadOnlyList<HandheldTerminalDto>>> GetAll(
        HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (companyCode, isAirport) = ctx.GetCompanyContext();

        var query = db.HandheldTerminalSet
            .AsNoTracking()
            .Include(h => h.AssignedCompany)
            .AsQueryable();

        // Handling agents see only their assigned terminals
        if (!isAirport)
            query = query.Where(h => h.AssignedCompanyCode == companyCode);

        var terminals = await query
            .OrderBy(h => h.DeviceId)
            .Select(h => ToDto(h))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<HandheldTerminalDto>>(terminals);
    }

    private static async Task<Results<Ok<HandheldTerminalDto>, NotFound>> GetById(
        int id, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (companyCode, isAirport) = ctx.GetCompanyContext();

        var hht = await db.HandheldTerminalSet
            .AsNoTracking()
            .Include(h => h.AssignedCompany)
            .FirstOrDefaultAsync(h => h.Id == id, ct);

        if (hht is null) return TypedResults.NotFound();
        if (!isAirport && hht.AssignedCompanyCode != companyCode) return TypedResults.NotFound();

        return TypedResults.Ok(ToDto(hht));
    }

    private static async Task<Results<Created<HandheldTerminalDto>, BadRequest<string>, ForbidHttpResult, Conflict<string>>> Create(
        CreateHandheldTerminalRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var exists = await db.HandheldTerminalSet
            .AnyAsync(h => h.DeviceId == request.DeviceId.ToUpperInvariant().Trim(), ct);
        if (exists) return TypedResults.Conflict($"Device ID '{request.DeviceId}' already exists.");

        var hht = HandheldTerminal.Create(request.DeviceId, request.Name, request.SerialNumber, request.Model);
        db.HandheldTerminalSet.Add(hht);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/hht/{hht.Id}", ToDto(hht));
    }

    private static async Task<Results<Ok<HandheldTerminalDto>, ForbidHttpResult, NotFound>> Update(
        int id, UpdateHandheldTerminalRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var hht = await db.HandheldTerminalSet.Include(h => h.AssignedCompany).FirstOrDefaultAsync(h => h.Id == id, ct);
        if (hht is null) return TypedResults.NotFound();

        hht.Update(request.Name, request.SerialNumber, request.Model);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(hht));
    }

    private static async Task<Results<Ok<HandheldTerminalDto>, BadRequest<string>, ForbidHttpResult, NotFound>> Assign(
        int id, AssignHandheldTerminalRequest request, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var hht = await db.HandheldTerminalSet.Include(h => h.AssignedCompany).FirstOrDefaultAsync(h => h.Id == id, ct);
        if (hht is null) return TypedResults.NotFound();

        var company = await db.CompanySet.FindAsync([request.HandlingCompanyCode], ct);
        if (company is null) return TypedResults.BadRequest("Handling company not found.");

        hht.AssignToCompany(request.HandlingCompanyCode);
        await db.SaveChangesAsync(ct);

        // Reload navigation
        await db.Entry(hht).Reference(h => h.AssignedCompany).LoadAsync(ct);
        return TypedResults.Ok(ToDto(hht));
    }

    private static async Task<Results<Ok<HandheldTerminalDto>, ForbidHttpResult, NotFound>> Unassign(
        int id, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        if (!ctx.IsAirportOperator()) return TypedResults.Forbid();

        var hht = await db.HandheldTerminalSet.Include(h => h.AssignedCompany).FirstOrDefaultAsync(h => h.Id == id, ct);
        if (hht is null) return TypedResults.NotFound();

        hht.Unassign();
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(hht));
    }   

    private static HandheldTerminalDto ToDto(HandheldTerminal h) => new(
        h.Id,
        h.DeviceId,
        h.Name,
        h.SerialNumber,
        h.Model,
        h.AssignedCompanyCode,
        h.AssignedCompany?.Name,
        h.AssignedAt,
        h.CreatedAt);
}
