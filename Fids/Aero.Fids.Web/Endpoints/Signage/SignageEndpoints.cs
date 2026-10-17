namespace Aero.Fids.Web.Endpoints.Signage;

public static class SignageEndpoints
{
    public static IEndpointRouteBuilder MapSignageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/signage/active", async (string tenant, IPublicSignageReader reader, CancellationToken ct) =>
            Results.Ok(await reader.GetActiveAsync(tenant, ct)));

        app.MapGet("/api/signage/{id:long}/image", async (long id, string tenant, IPublicSignageReader reader, HttpContext ctx, CancellationToken ct) =>
        {
            var image = await reader.GetImageAsync(tenant, id, ct);
            if (image is null)
                return Results.NotFound();

            ctx.Response.Headers.CacheControl = "public, max-age=3600";
            return Results.File(image.Value.Data, image.Value.ContentType);
        });

        return app;
    }
}
