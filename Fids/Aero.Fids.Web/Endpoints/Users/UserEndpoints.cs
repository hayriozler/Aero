using Aero.Fids.Web.Services;

namespace Aero.Fids.Web.Endpoints.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/fids-users").RequireAuthorization();

        group.MapGet("/", async (IFidsUserService service, CancellationToken ct) =>
            Results.Ok(await service.ListAsync(ct)));

        group.MapPost("/", async (CreateFidsUserRequest request, IFidsUserService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(request, ct);
                return Results.Created($"/api/fids-users/{created.Id}", created);
            }
            catch (FidsValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPut("/{id:int}", async (int id, UpdateFidsUserRequest request, IFidsUserService service, CancellationToken ct) =>
        {
            try
            {
                var updated = await service.UpdateAsync(id, request, ct);
                return updated is null
                    ? Results.NotFound(new { error = $"User {id} not found." })
                    : Results.Ok(updated);
            }
            catch (FidsValidationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapDelete("/{id:int}", async (int id, IFidsUserService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(id, ct);
            return deleted
                ? Results.NoContent()
                : Results.NotFound(new { error = $"User {id} not found." });
        });

        return app;
    }
}
