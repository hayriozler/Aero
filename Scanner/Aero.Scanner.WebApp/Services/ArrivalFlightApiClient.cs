using Aero.Scanner.Shared.Dtos;

namespace Aero.Scanner.WebApp.Services;

public sealed class ArrivalFlightApiClient(HttpClient client) : FlightApiClientBase
{
    public async Task<IReadOnlyList<ArrivalFlightDto>> GetArrivalFlightsAsync(
        bool includeAll = false,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl("/api/arrival/flights", includeAll, from, to);
            var result = await client.GetFromJsonAsync<List<ArrivalFlightDto>>(url, ct);
            return result ?? [];
        }
        catch
        {
            return [];
        }
    }

}
