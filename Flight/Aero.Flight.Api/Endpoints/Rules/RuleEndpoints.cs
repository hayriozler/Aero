using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Application.Common;
using Aero.Flights.Application.Rules;
using Aero.Flights.Contracts.Rules;
using Aero.Flights.Domain.Identity;
using FluentValidation;

namespace Aero.Flights.Api.Endpoints.Rules;

public static class RuleEndpoints
{
    public static IEndpointRouteBuilder MapRuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rules")
            .WithTags("Rules")
            .RequireAuthorization(Permissions.RuleManage);

        group.MapGet("/", async (
            IQueryHandler<GetRulesQuery, IReadOnlyList<RuleResponse>> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetRulesQuery(), ct)));

        group.MapGet("/metadata", async (
            IQueryHandler<GetRuleMetadataQuery, RuleMetadataResponse> handler,
            HttpContext ctx,
            CancellationToken ct) =>
            ApiResult.Ok(ctx, await handler.HandleAsync(new GetRuleMetadataQuery(), ct)));

        group.MapGet("/{id:int}", async (
            int id,
            IQueryHandler<GetRuleQuery, RuleResponse?> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetRuleQuery(id), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "RULE_NOT_FOUND", $"Rule {id} not found.")
                : ApiResult.Ok(ctx, result);
        });

        group.MapPost("/", async (
            RuleRequest request,
            ICommandHandler<CreateRuleCommand, RuleResponse> handler,
            IValidator<RuleRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new CreateRuleCommand(request), ct);
            return ApiResult.Created(ctx, result, $"/api/rules/{result.Id}");
        });

        group.MapPut("/{id:int}", async (
            int id,
            RuleRequest request,
            ICommandHandler<UpdateRuleCommand, RuleResponse?> handler,
            IValidator<RuleRequest> validator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResult.ValidationFailed(ctx, validation);

            var result = await handler.HandleAsync(new UpdateRuleCommand(id, request), ct);
            return result is null
                ? ApiResult.NotFound(ctx, "RULE_NOT_FOUND", $"Rule {id} not found.")
                : ApiResult.Ok(ctx, result);
        });

        group.MapDelete("/{id:int}", async (
            int id,
            ICommandHandler<DeleteRuleCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(new DeleteRuleCommand(id), ct);
            return deleted
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "RULE_NOT_FOUND", $"Rule {id} not found.");
        });

        group.MapPut("/{id:int}/enabled", async (
            int id,
            RuleSetEnabledRequest request,
            ICommandHandler<SetRuleEnabledCommand, bool> handler,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var found = await handler.HandleAsync(new SetRuleEnabledCommand(id, request.IsEnabled), ct);
            return found
                ? ApiResult.NoContent(ctx)
                : ApiResult.NotFound(ctx, "RULE_NOT_FOUND", $"Rule {id} not found.");
        });

        return app;
    }
}
