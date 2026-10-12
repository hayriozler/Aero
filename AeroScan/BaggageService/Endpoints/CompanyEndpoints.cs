using BaggageService.Filters;
using BaggageService.Persistence;
using Contracts.Consts;
using Infrastructure.Extensions;
using Contracts.Dtos;
using Contracts.Requests;
using Domain.Aggregates.Companies;
using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/companies")
            .RequireAuthorization(Policies.AirportOrRamp);

        group.MapGet("/", GetAll)
            .WithName("GetAllCompanies")
            .Produces<IReadOnlyList<CompanyDto>>();

        group.MapGet("/{code}", GetByCode)
            .WithName("GetCompanyByCode")
            .Produces<CompanyDto>()
            .ProducesProblem(404);

        group.MapPost("/", Create)
            .WithName("CreateCompany")
            .AddEndpointFilter<ValidationFilter<CreateCompanyRequest>>()
            .Produces<CompanyDto>(201)
            .ProducesProblem(400)
            .ProducesProblem(409);

        group.MapPut("/{code}", Update)
            .WithName("UpdateCompany")
            .AddEndpointFilter<ValidationFilter<UpdateCompanyRequest>>()
            .Produces<CompanyDto>()
            .ProducesProblem(400)
            .ProducesProblem(404);

        group.MapDelete("/{code}", Deactivate)
            .WithName("DeactivateCompany")
            .Produces(204)
            .ProducesProblem(404);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<CompanyDto>>> GetAll(
        HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (callerCompanyCode, isAirport) = ctx.GetCompanyContext();

        var query = db.CompanySet.AsNoTracking().AsQueryable();
        if (!isAirport)
            query = query.Where(c => c.Code == callerCompanyCode);

        var companies = await query.OrderBy(c => c.Name).Select(c => ToDto(c)).ToListAsync(ct);
        return TypedResults.Ok<IReadOnlyList<CompanyDto>>(companies);
    }

    private static async Task<Results<Ok<CompanyDto>, NotFound>> GetByCode(
        string code, HttpContext ctx, AeroScanDataContext db, CancellationToken ct)
    {
        var (callerCompanyCode, isAirport) = ctx.GetCompanyContext();

        var company = await db.CompanySet.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code, ct);
        if (company is null) return TypedResults.NotFound();
        if (!isAirport && company.Code != callerCompanyCode) return TypedResults.NotFound();

        return TypedResults.Ok(ToDto(company));
    }

    private static async Task<Results<Created<CompanyDto>, BadRequest<string>, Conflict<string>>> Create(
        CreateCompanyRequest request, AeroScanDataContext db, CancellationToken ct)
    {
        var type = Enum.Parse<CompanyType>(request.Type, ignoreCase: true);

        var exists = await db.CompanySet.AnyAsync(c => c.Code.Equals(request.Code), ct);
        if (exists)
            return TypedResults.Conflict($"A company with code '{request.Code}' already exists.");

        var company = Company.Create(request.Name, request.Code, type);
        db.CompanySet.Add(company);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/companies/{company.Code}", ToDto(company));
    }

    private static async Task<Results<Ok<CompanyDto>, BadRequest<string>, NotFound>> Update(
        string code, UpdateCompanyRequest request, AeroScanDataContext db, CancellationToken ct)
    {
        var type = Enum.Parse<CompanyType>(request.Type, ignoreCase: true);

        var company = await db.CompanySet.FirstOrDefaultAsync(c => c.Code == code, ct);
        if (company is null) return TypedResults.NotFound();

        company.Update(request.Name, request.Code, type);
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(company));
    }

    private static async Task<Results<NoContent, NotFound>> Deactivate(
        string code, AeroScanDataContext db, CancellationToken ct)
    {
        var company = await db.CompanySet.FirstOrDefaultAsync(c => c.Code == code, ct);
        if (company is null) return TypedResults.NotFound();

        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }


    private static CompanyDto ToDto(Company c) =>
        new(c.Code, c.Name, c.Type.ToString(), c.CreatedAt);
}
