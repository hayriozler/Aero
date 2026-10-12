using Aero.Scanner.Shared.Dtos;

namespace Aero.Scanner.WebApp.Services;

public sealed class DepartureBagApiClient(HttpClient client)
{
    public async Task<IReadOnlyList<DepartureBagDto>> GetBagsForFlightAsync(int flightId, CancellationToken ct = default)
    {
        try
        {
            var result = await client.GetFromJsonAsync<List<DepartureBagDto>>(
                $"/api/bags?flightKey={flightId}", ct);
            return result ?? [];
        }
        catch
        {
            return [];  
        }
    }
}
