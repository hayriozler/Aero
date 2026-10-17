namespace Aero.Flights.Application.Metadatas;

public static class ImageUploadValidation
{
    public const long MaxSizeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> _allowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/jpeg", "image/webp", "image/gif", "image/svg+xml"
    };

    public static string? Validate(long sizeBytes, string contentType)
    {
        if (sizeBytes == 0)
            return "The uploaded file is empty.";
        if (sizeBytes > MaxSizeBytes)
            return $"The uploaded file exceeds the maximum size of {MaxSizeBytes / (1024 * 1024)} MB.";
        if (!_allowedContentTypes.Contains(contentType))
            return $"Content type '{contentType}' is not supported. Allowed types: {string.Join(", ", _allowedContentTypes)}.";
        return null;
    }
}
