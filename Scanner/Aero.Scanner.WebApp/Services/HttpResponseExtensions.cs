using System.Net;
using System.Text.Json;

namespace Aero.Scanner.WebApp.Services;

/// <summary>
/// Parses Problem Details (RFC 9457) responses from the API into user-friendly error messages.
/// </summary>
internal static class HttpResponseExtensions
{
    /// <summary>
    /// Reads the response body and extracts a friendly error message.
    /// — 400 with "errors": joins all validation messages.
    /// — 409/404: returns the "detail" field.
    /// — 500+: returns a generic message (never shows internal exception details).
    /// </summary>
    internal static async Task<string> ReadApiErrorAsync(
        this HttpResponseMessage response, CancellationToken ct = default)
    {
        var body = await response.Content.ReadAsStringAsync(ct);

        // 5xx — never expose internals
        if ((int)response.StatusCode >= 500)
            return "An unexpected server error occurred. Please try again.";

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // 400 Validation Problem — collect all field errors
            if (response.StatusCode == HttpStatusCode.BadRequest &&
                root.TryGetProperty("errors", out var errors))
            {
                var messages = new List<string>();
                foreach (var field in errors.EnumerateObject())
                    foreach (var msg in field.Value.EnumerateArray())
                    {
                        var text = msg.GetString();
                        if (!string.IsNullOrWhiteSpace(text))
                            messages.Add(text);
                    }

                if (messages.Count > 0)
                    return string.Join(" ", messages);
            }

            // detail field (409 Conflict, 404, etc.)
            if (root.TryGetProperty("detail", out var detail))
            {
                var text = detail.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                    return text;
            }

            // title fallback
            if (root.TryGetProperty("title", out var title))
            {
                var text = title.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                    return text;
            }
        }
        catch { }

        return "An error occurred. Please try again.";
    }
}
