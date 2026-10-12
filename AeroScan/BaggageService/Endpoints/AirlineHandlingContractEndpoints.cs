using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Infrastructure.Extensions;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.HandlingContracts;
using Domain.Enums;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class AirlineHandlingContractEndpoints
{
    public static IEndpointRouteBuilder MapHandlingContractEndpoints(this IEndpointRouteBuilder app)
    {
        // Both airport-operator admins/supervisors AND handling-agent supervisors can access.
        // Company-level scoping is enforced inside each handler.
        var group = app.MapGroup("/api/handling-contracts")
            .RequireAuthorization(Policies.RamSupervisor);

        group.MapGet("/", GetAll)
            .WithName("GetAllHandlingContracts")
            .Produces<IReadOnlyList<AirlineHandlingContractDto>>();

        group.MapGet("/{id:int}", GetById)
            .WithName("GetHandlingContractById")
            .Produces<AirlineHandlingContractDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .WithName("CreateHandlingContract")
            .AddEndpointFilter<ValidationFilter<CreateAirlineHandlingContractRequest>>()
            .Produces<AirlineHandlingContractDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(409);

        group.MapPut("/{id:int}", Update)
            .WithName("UpdateHandlingContract")
            .AddEndpointFilter<ValidationFilter<UpdateAirlineHandlingContractRequest>>()
            .Produces<AirlineHandlingContractDto>()
            .ProducesProblem(400)
            .ProducesProblem(404);
       

        return app;
    }

    // ── Handlers ─────────────────────────────────────────────────────────────

    private static async Task<Ok<IReadOnlyList<AirlineHandlingContractDto>>> GetAll(
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userCompanyCode = httpContext.GetCompanyCode();
        var isHandlingAgent = httpContext.IsHandlingAgent();

        var query = db.HandlingContractSet
            .Include(c => c.HandlingCompany)
            .Include(c => c.FlightNumbers)
            .AsNoTracking()
            .AsQueryable();

        // Handling agents see only their own company's contracts
        if (isHandlingAgent)
            query = query.Where(c => c.HandlingCompanyCode == userCompanyCode);

        var contracts = await query
            .OrderBy(c => c.AirlineCode)
            .ThenBy(c => c.ValidFrom)
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<AirlineHandlingContractDto>>(
            [.. contracts.Select(ToDto)]);
    }

    private static async Task<Results<Ok<AirlineHandlingContractDto>, NotFound>> GetById(
        int id,
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userCompanyCode = httpContext.GetCompanyCode();
        var isHandlingAgent = httpContext.IsHandlingAgent();

        var contract = await db.HandlingContractSet
            .Include(c => c.HandlingCompany)
            .Include(c => c.FlightNumbers)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (contract is null) return TypedResults.NotFound();

        // Handling agents cannot see other companies' contracts
        if (isHandlingAgent && contract.HandlingCompanyCode != userCompanyCode)
            return TypedResults.NotFound();

        return TypedResults.Ok(ToDto(contract));
    }

    private static async Task<Results<Created<AirlineHandlingContractDto>, BadRequest<string>, Conflict<string>>> Create(
        CreateAirlineHandlingContractRequest request,
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userCompanyCode = httpContext.GetCompanyCode();
        var isHandlingAgent = httpContext.IsHandlingAgent();

        // Handling agents can only create contracts for their own company
        var effectiveCompanyCode = isHandlingAgent ? userCompanyCode : request.HandlingCompanyCode;

        var company = await db.CompanySet.FindAsync([effectiveCompanyCode], ct);
        if (company is null)
            return TypedResults.BadRequest($"Company with id {effectiveCompanyCode} not found.");
        if (company.Type != CompanyType.HandlingAgent)
            return TypedResults.BadRequest("The selected company must be a HandlingAgent.");

        var airlineCode = request.AirlineCode.ToUpperInvariant().Trim();

        var overlap = await db.HandlingContractSet
            .AnyAsync(c => c.IsActive
                        && c.HandlingCompanyCode == effectiveCompanyCode
                        && c.AirlineCode       == airlineCode
                        && c.ValidFrom         < request.ValidTo
                        && c.ValidTo           > request.ValidFrom, ct);

        if (overlap)
            return TypedResults.Conflict(
                $"An active contract for airline '{airlineCode}' already exists within the requested date range for this company.");

        var contract = AirlineHandlingContract.Create(
            airlineCode,
            effectiveCompanyCode,
            request.ValidFrom,
            request.ValidTo,
            request.Notes);

        db.HandlingContractSet.Add(contract);
        await db.SaveChangesAsync(ct); // Save first to get the generated Id

        if (request.FlightNumbers.Count > 0)
        {
            contract.SetFlightNumbers(request.FlightNumbers);
            await db.SaveChangesAsync(ct);
        }

        await db.Entry(contract).Reference(c => c.HandlingCompany).LoadAsync(ct);

        return TypedResults.Created($"/api/handling-contracts/{contract.Id}", ToDto(contract));
    }

    private static async Task<Results<Ok<AirlineHandlingContractDto>, BadRequest<string>, NotFound>> Update(
        int id,
        UpdateAirlineHandlingContractRequest request,
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userCompanyCode = httpContext.GetCompanyCode();
        var isHandlingAgent = httpContext.IsHandlingAgent();

        var contract = await db.HandlingContractSet
            .Include(c => c.HandlingCompany)
            .Include(c => c.FlightNumbers)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (contract is null) return TypedResults.NotFound();

        // Handling agents can only modify their own contracts
        if (isHandlingAgent && contract.HandlingCompanyCode != userCompanyCode)
            return TypedResults.NotFound();

        // Handling agents cannot reassign the contract to a different company
        var effectiveCompanyCode = isHandlingAgent ? userCompanyCode : request.HandlingCompanyCode;

        var company = await db.CompanySet.FindAsync([effectiveCompanyCode], ct);
        if (company is null)
            return TypedResults.BadRequest($"Company with id {effectiveCompanyCode} not found.");
        if (company.Type != CompanyType.HandlingAgent)
            return TypedResults.BadRequest("The selected company must be a HandlingAgent.");

        contract.Update(
            request.AirlineCode,
            effectiveCompanyCode,
            request.ValidFrom,
            request.ValidTo,
            request.Notes);

        contract.SetFlightNumbers(request.FlightNumbers);

        await db.SaveChangesAsync(ct);
        await db.Entry(contract).Reference(c => c.HandlingCompany).LoadAsync(ct);

        return TypedResults.Ok(ToDto(contract));
    }

    private static async Task<Results<NoContent, NotFound>> Deactivate(
        int id,
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userCompanyCode = httpContext.GetCompanyCode();
        var isHandlingAgent = httpContext.IsHandlingAgent();

        var contract = await db.HandlingContractSet.FindAsync([id], ct);
        if (contract is null) return TypedResults.NotFound();

        // Handling agents can only deactivate their own contracts
        if (isHandlingAgent && contract.HandlingCompanyCode != userCompanyCode)
            return TypedResults.NotFound();

        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    // ── Mapping ───────────────────────────────────────────────────────────────

    private static AirlineHandlingContractDto ToDto(AirlineHandlingContract c) => new(
        Id:                  c.Id,
        AirlineCode:         c.AirlineCode,
        HandlingCompanyName: c.HandlingCompany.Name,
        HandlingCompanyCode: c.HandlingCompany.Code,
        ValidFrom:           c.ValidFrom,
        ValidTo:             c.ValidTo,
        IsActive:            c.IsActive,
        Notes:               c.Notes,
        FlightNumbers:       [.. c.FlightNumbers.Select(fn => fn.FlightNumber).OrderBy(fn => fn)],
        CreatedAt:           c.CreatedAt,
        UpdatedAt:           c.UpdatedAt);
}
