using AeroFlight.Api.Infrastructure;
using AeroFlight.Application.Common;
using AeroFlight.Application.Metadatas;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Identity;
using FluentValidation;

namespace AeroFlight.Api.Endpoints.Metadatas;

public static class CompanyContactEndpoints
{
    public static IEndpointRouteBuilder MapCompanyContactEndpoints(this IEndpointRouteBuilder app)
    {
        const string basePath = "/api/metadata/companies/{code}/contacts";
        var group = app.MapGroup(basePath).WithTags("Metadata: Company Contacts");

        group.MapGet("/", async (
            string code,
            IQueryHandler<ListCompanyContactsQuery, IReadOnlyList<CompanyContactResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new ListCompanyContactsQuery(code), ct)))
            .RequireAuthorization(Permissions.MetadataView);

        group.MapPost("/", async (
            string code,
            CompanyContactRequest request,
            ICommandHandler<AddCompanyContactCommand, CompanyContactResponse> handler,
            IValidator<CompanyContactRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new AddCompanyContactCommand(code, request), ct);
            return ApiResult.Created(ctx, result, $"{basePath.Replace("{code}", code)}/{result.Id}");
        }).RequireAuthorization(Permissions.MetadataCreate);

        group.MapPut("/{id:int}", async (
            string code,
            int id,
            CompanyContactRequest request,
            ICommandHandler<UpdateCompanyContactCommand, CompanyContactResponse?> handler,
            IValidator<CompanyContactRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateCompanyContactCommand(code, id, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "COMPANY_CONTACT_NOT_FOUND", $"Company contact {id} not found.")
                : ApiResult.Ok(ctx, result);
        }).RequireAuthorization(Permissions.MetadataUpdate);

        group.MapDelete("/{id:int}", async (
            string code,
            int id,
            ICommandHandler<DeleteCompanyContactCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteCompanyContactCommand(code, id), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "COMPANY_CONTACT_NOT_FOUND", $"Company contact {id} not found.");
        }).RequireAuthorization(Permissions.MetadataDelete);

        return app;
    }
}
