using Aero.Fids.Web.Services;

namespace Aero.Fids.Web.Endpoints.FieldMappings;

public sealed record FieldMappingRequest(string SourcePath);
public sealed record FieldMappingRuleRequest(string? RuleOperator, string? RuleValue);

public static class FieldMappingEndpoints
{
    public static IEndpointRouteBuilder MapFieldMappingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/field-mappings").RequireAuthorization();

        group.MapGet("/", async (IFieldMappingService service, CancellationToken ct) =>
            Results.Ok(await service.ListAsync(ct)));

        // ?flightType=Arrival|Departure sets a type-specific override; omitted sets the default.
        group.MapPut("/{targetField}", async (
            string targetField, string? flightType, FieldMappingRequest request, IFieldMappingService service, CancellationToken ct) =>
        {
            try
            {
                return Results.Ok(await service.SetAsync(targetField, flightType, request.SourcePath, ct));
            }
            catch (FidsValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPut("/{targetField}/rule", async (
            string targetField, string? flightType, FieldMappingRuleRequest request, IFieldMappingService service, CancellationToken ct) =>
        {
            try
            {
                return Results.Ok(await service.SetRuleAsync(targetField, flightType, request.RuleOperator, request.RuleValue, ct));
            }
            catch (FidsValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapDelete("/{targetField}", async (string targetField, string? flightType, IFieldMappingService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(targetField, flightType, ct);
            return deleted
                ? Results.NoContent()
                : Results.NotFound(new { error = $"No mapping set for '{targetField}'" + (flightType is null ? "." : $" ({flightType}).") });
        });

        return app;
    }
}
