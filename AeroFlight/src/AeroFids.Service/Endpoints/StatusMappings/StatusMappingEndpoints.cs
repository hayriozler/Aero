using AeroFids.Service.Services;

namespace AeroFids.Service.Endpoints.StatusMappings;

public sealed record StatusMappingRequest(string InternalValue);

public static class StatusMappingEndpoints
{
    public static IEndpointRouteBuilder MapStatusMappingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/status-mappings").RequireAuthorization();

        group.MapGet("/", async (IStatusMappingService service, CancellationToken ct) =>
            Results.Ok(await service.ListAsync(ct)));

        // ?flightType=Arrival|Departure only applies to the Flight dimension.
        group.MapPut("/{dimension}/{externalValue}", async (
            string dimension, string externalValue, string? flightType, StatusMappingRequest request,
            IStatusMappingService service, CancellationToken ct) =>
        {
            try
            {
                return Results.Ok(await service.SetAsync(dimension, flightType, externalValue, request.InternalValue, ct));
            }
            catch (FidsValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapDelete("/{dimension}/{externalValue}", async (
            string dimension, string externalValue, string? flightType, IStatusMappingService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(dimension, flightType, externalValue, ct);
            return deleted
                ? Results.NoContent()
                : Results.NotFound(new { error = $"No status mapping set for {dimension}/{externalValue}" + (flightType is null ? "." : $" ({flightType}).") });
        });

        return app;
    }
}
