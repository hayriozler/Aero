using Aero.Scanner.Shared.Dtos;

namespace Aero.Scanner.WebApp.Services;

public sealed class AuditApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<AuditEntryDto>> GetHistoryAsync(
        string entityType,
        string pk,
        CancellationToken ct = default)
    {
        var url = $"/api/audits/{Uri.EscapeDataString(entityType)}?pk={Uri.EscapeDataString(pk)}";
        var response = await http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<AuditEntryDto>>(ct) ?? [];
    }
}
