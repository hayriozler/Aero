using Aero.Scanner.Shared.Dtos;

namespace Aero.Scanner.WebApp.Services;
public sealed class DepartureFlightApiClient(HttpClient client) : FlightApiClientBase
{
    public async Task<IReadOnlyList<DepartureFlightDto>> GetFlightsAsync(
        bool includeAll = false,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl("/api/departure/flights", includeAll, from, to);
            var result = await client.GetFromJsonAsync<List<DepartureFlightDto>>(url, ct);
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }
}
