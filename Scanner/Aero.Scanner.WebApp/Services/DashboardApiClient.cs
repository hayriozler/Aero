using Aero.Scanner.Shared.Dtos;

namespace Aero.Scanner.WebApp.Services;

public sealed class DashboardApiClient(HttpClient client)
{
    public async Task<DashboardSummaryDto?> GetSummaryAsync(CancellationToken ct = default)
    {
        try
        {
            return await client.GetFromJsonAsync<DashboardSummaryDto>("/api/dashboard/summary", ct);
        }
        catch
        {
            return null;
        }
    }
}
