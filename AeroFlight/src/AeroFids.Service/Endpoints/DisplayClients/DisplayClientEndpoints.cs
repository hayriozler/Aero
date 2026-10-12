using AeroFids.Service.Services;

namespace AeroFids.Service.Endpoints.DisplayClients;

public static class DisplayClientEndpoints
{
    public static IEndpointRouteBuilder MapDisplayClientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/display-clients").RequireAuthorization();

        group.MapGet("/", async (IDisplayClientService service, CancellationToken ct) =>
            Results.Ok(await service.ListAsync(ct)));

        group.MapPost("/", async (DisplayClientRequest request, IDisplayClientService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(request, ct);
                return Results.Created($"/api/display-clients/{created.Id}", created);
            }
            catch (FidsValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPut("/{id:long}", async (long id, DisplayClientRequest request, IDisplayClientService service, CancellationToken ct) =>
        {
            try
            {
                var updated = await service.UpdateAsync(id, request, ct);
                return updated is null
                    ? Results.NotFound(new { error = $"Display client {id} not found." })
                    : Results.Ok(updated);
            }
            catch (FidsValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapDelete("/{id:long}", async (long id, IDisplayClientService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(id, ct);
            return deleted
                ? Results.NoContent()
                : Results.NotFound(new { error = $"Display client {id} not found." });
        });

        return app;
    }
}
