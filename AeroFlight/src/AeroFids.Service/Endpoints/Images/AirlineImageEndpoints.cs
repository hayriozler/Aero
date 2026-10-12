using System.Text.RegularExpressions;

namespace AeroFids.Service.Endpoints.Images;

// A tenant admin (or IT) copies logo files to AirlineImagesBasePath/{code}/{size}.{ext} —
// AeroFlight's own metadata is a separate external system and is never touched for this.
public sealed record FidsImageOptions(string? AirlineImagesBasePath);

public static class AirlineImageEndpoints
{
    private static readonly Regex _segmentPattern = new("^[A-Za-z0-9_-]{1,20}$", RegexOptions.Compiled);
    private static readonly string[] _extensions = [".png", ".jpg", ".jpeg", ".svg", ".webp"];

    public static IEndpointRouteBuilder MapAirlineImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/images/airlines/{code}/{size}", (string code, string size, FidsImageOptions options, HttpContext ctx) =>
        {
            if (string.IsNullOrWhiteSpace(options.AirlineImagesBasePath) || !_segmentPattern.IsMatch(code) || !_segmentPattern.IsMatch(size))
                return Results.NotFound();

            foreach (var ext in _extensions)
            {
                var path = Path.Combine(options.AirlineImagesBasePath, code.ToUpperInvariant(), size.ToLowerInvariant() + ext);
                if (!File.Exists(path))
                    continue;

                ctx.Response.Headers.CacheControl = "public, max-age=43200";
                return Results.File(path, ContentTypeFor(ext));
            }

            return Results.NotFound();
        });

        return app;
    }

    private static string ContentTypeFor(string ext) => ext switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".svg" => "image/svg+xml",
        ".webp" => "image/webp",
        _ => "application/octet-stream",
    };
}
