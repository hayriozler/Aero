using System.Text.Json;

namespace Aero.Flights.Web.Services.Metadatas;

public sealed record MetadataPage(IReadOnlyList<IReadOnlyDictionary<string, JsonElement>> Items, int TotalCount);

public sealed record AuditEntryDto(long Id, string Action, string ChangedBy, DateTime Timestamp, string Snapshot);

public interface IMetadataClient
{
    Task<IReadOnlyList<IReadOnlyDictionary<string, JsonElement>>> GetAllAsync(string route, CancellationToken ct = default);
    Task<MetadataPage> GetPageAsync(string route, int page, int pageSize, string? search, CancellationToken ct = default);
    Task CreateAsync(string route, IReadOnlyDictionary<string, object?> body, CancellationToken ct = default);
    Task UpdateAsync(string route, string id, IReadOnlyDictionary<string, object?> body, CancellationToken ct = default);
    Task<bool> DeleteAsync(string route, string id, CancellationToken ct = default);
    Task<IReadOnlyList<AuditEntryDto>> GetAuditAsync(string route, string id, CancellationToken ct = default);
}

// No client-side caching here — the API caches Metadata-data List/Get itself (long-lived for pure
// lookups, briefly for CRUD entities), so every consumer of the API benefits, not just this app.
public sealed class MetadataDataClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IMetadataClient
{
    public async Task<IReadOnlyList<IReadOnlyDictionary<string, JsonElement>>> GetAllAsync(string route, CancellationToken ct = default)
    {
        using var request = CreateRequest(HttpMethod.Get, route);
        using var response = await Http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>(JsonOptions, ct);
        return ToRows(envelope?.Data);
    }

    public async Task<MetadataPage> GetPageAsync(string route, int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var uri = $"{route}?page={page + 1}&pageSize={pageSize}";
        using var request = CreateRequest(HttpMethod.Get, uri);
        using var response = await Http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>(JsonOptions, ct);
        var rows = ToRows(envelope?.Data);

        // Server-side paging when the API supplies pagination metadata.
        if (envelope?.Pagination is { } p)
            return new MetadataPage(rows, p.TotalCount);

        // Fallback: API returned the full list — filter and page on the client.
        IEnumerable<IReadOnlyDictionary<string, JsonElement>> query = rows;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(r => r.Values.Any(v =>
                v.ValueKind == JsonValueKind.String
                && (v.GetString()?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)));
        }

        var filtered = query.ToList();
        var items = filtered.Skip(page * pageSize).Take(pageSize).ToList();
        return new MetadataPage(items, filtered.Count);
    }

    public Task CreateAsync(string route, IReadOnlyDictionary<string, object?> body, CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, route, body, ct);

    public Task UpdateAsync(string route, string id, IReadOnlyDictionary<string, object?> body, CancellationToken ct = default)
        => SendAsync(HttpMethod.Put, $"{route}/{Uri.EscapeDataString(id)}", body, ct);

    public Task<bool> DeleteAsync(string route, string id, CancellationToken ct = default)
        => DeleteAsync($"{route}/{Uri.EscapeDataString(id)}", ct);

    public Task<IReadOnlyList<AuditEntryDto>> GetAuditAsync(string route, string id, CancellationToken ct = default)
        => GetListAsync<AuditEntryDto>($"{route}/{Uri.EscapeDataString(id)}/audit", ct);

    private static List<IReadOnlyDictionary<string, JsonElement>> ToRows(JsonElement? data)
    {
        var rows = new List<IReadOnlyDictionary<string, JsonElement>>();
        if (data is not { ValueKind: JsonValueKind.Array } array)
            return rows;

        foreach (var element in array.EnumerateArray())
        {
            var row = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in element.EnumerateObject())
                row[property.Name] = property.Value;
            rows.Add(row);
        }

        return rows;
    }
}
