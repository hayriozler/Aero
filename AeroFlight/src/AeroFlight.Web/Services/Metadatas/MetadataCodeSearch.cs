using System.Text.Json;

namespace AeroFlight.Web.Services.Metadatas;

// Shared MudAutocomplete SearchFunc backing for "pick a Metadata row by code" fields (payer
// company, handling agent, handling service, ...) — matches search text against every string
// field on the row (not just code/name), same behavior FlightServiceEditDialog established first.
internal static class MetadataCodeSearch
{
    public static async Task<IEnumerable<string>> SearchAsync(
        IMetadataClient Metadata, string route, string search, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return [];

        try
        {
            var rows = await Metadata.GetAllAsync(route, ct);
            IEnumerable<IReadOnlyDictionary<string, JsonElement>> matches = rows;

            if (!string.IsNullOrWhiteSpace(search))
                matches = matches.Where(row => RowMatches(row, search));

            return matches
                .Select(row => row.TryGetValue("code", out var code) && code.ValueKind == JsonValueKind.String ? code.GetString() : null)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Cast<string>()
                .Take(20);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private static bool RowMatches(IReadOnlyDictionary<string, JsonElement> row, string search)
    {
        foreach (var value in row.Values)
        {
            if (value.ValueKind == JsonValueKind.String)
            {
                var s = value.GetString();
                if (!string.IsNullOrEmpty(s) && s.Contains(search, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }

    // Same as SearchAsync, but also fills in a "Code — label" string per returned code (for a
    // MudAutocomplete ItemTemplate) so the dropdown can show more than the bare code. Populates
    // `labels` as a side effect rather than returning pairs, matching how ItemTemplate/ToStringFunc
    // need a code-keyed lookup rather than a list.
    public static async Task<IEnumerable<string>> SearchWithLabelsAsync(
        IMetadataClient Metadata, string route, string search, Dictionary<string, string> labels, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return [];

        try
        {
            var rows = await Metadata.GetAllAsync(route, ct);
            IEnumerable<IReadOnlyDictionary<string, JsonElement>> matches = rows;

            if (!string.IsNullOrWhiteSpace(search))
                matches = matches.Where(row => RowMatches(row, search));

            var codes = new List<string>();
            foreach (var row in matches)
            {
                if (!row.TryGetValue("code", out var codeElement) || codeElement.ValueKind != JsonValueKind.String)
                    continue;

                var code = codeElement.GetString();
                if (string.IsNullOrWhiteSpace(code))
                    continue;

                labels[code] = Label(row, code);
                codes.Add(code);
                if (codes.Count >= 20)
                    break;
            }

            return codes;
        }
        catch (Exception)
        {
            return [];
        }
    }

    // Reference tables spell their display text differently — "description" (Terminal, FlightStatus,
    // CountryType, ...), "name"/"displayName" (Airport, Airline), or neither (AircraftType, which
    // only has Manufacturer/Model) — try every convention before falling back to the bare code.
    private static readonly string[] _labelFields = ["description", "displayName", "name"];

    private static string Label(IReadOnlyDictionary<string, JsonElement> row, string code)
    {
        foreach (var key in _labelFields)
        {
            if (row.TryGetValue(key, out var element) && element.ValueKind == JsonValueKind.String)
            {
                var text = element.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                    return $"{code} — {text}";
            }
        }

        if (row.TryGetValue("manufacturer", out var manufacturerElement) && row.TryGetValue("model", out var modelElement))
        {
            var manufacturer = manufacturerElement.ValueKind == JsonValueKind.String ? manufacturerElement.GetString() : null;
            var model = modelElement.ValueKind == JsonValueKind.String ? modelElement.GetString() : null;
            var combined = string.Join(' ', new[] { manufacturer, model }.Where(s => !string.IsNullOrWhiteSpace(s)));
            if (!string.IsNullOrWhiteSpace(combined))
                return $"{code} — {combined}";
        }

        return code;
    }
}
