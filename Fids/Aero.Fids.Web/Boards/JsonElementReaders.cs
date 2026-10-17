using System.Text.Json;

namespace Aero.Fids.Web.Boards;

// Typed pulls off an already-resolved JsonElement — read defensively, a source message's shape
// is never guaranteed to match what a tenant's mapping expects.
internal static class JsonElementReaders
{
    public static string? GetString(JsonElement element) =>
        element.ValueKind == JsonValueKind.String ? element.GetString() : null;

    public static long? GetLong(JsonElement element) =>
        element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var value) ? value : null;

    public static DateTimeOffset? GetDateTimeOffset(JsonElement element) =>
        element.ValueKind == JsonValueKind.String && element.TryGetDateTimeOffset(out var value) ? value : null;

    public static DateOnly? GetDateOnly(JsonElement element) =>
        element.ValueKind == JsonValueKind.String && DateOnly.TryParse(element.GetString(), out var value) ? value : null;

    public static IReadOnlyList<string> GetMultiValue(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Array => [.. element.EnumerateArray().Select(GetString).Where(s => s is not null).Select(s => s!)],
        JsonValueKind.String => (element.GetString() ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
        _ => [],
    };
}
