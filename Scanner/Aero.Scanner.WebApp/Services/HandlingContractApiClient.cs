using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.WebApp.Services;

public sealed class HandlingContractApiClient(HttpClient client)
{
    public async Task<IReadOnlyList<AirlineHandlingContractDto>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await client.GetFromJsonAsync<List<AirlineHandlingContractDto>>(
                "/api/handling-contracts", ct);
            return result ?? [];
        }
        catch { return []; }
    }

    public async Task<AirlineHandlingContractDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await client.GetFromJsonAsync<AirlineHandlingContractDto>(
                $"/api/handling-contracts/{id}", ct);
        }
        catch { return null; }
    }

    public async Task<(bool Success, AirlineHandlingContractDto? Result, string? Error)> CreateAsync(
        CreateAirlineHandlingContractRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await client.PostAsJsonAsync("/api/handling-contracts", request, ct);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<AirlineHandlingContractDto>(ct);
                return (true, dto, null);
            }
            var error = await response.ReadApiErrorAsync(ct);
            return (false, null, error);
        }
        catch (Exception ex) { return (false, null, ex.Message); }
    }

    public async Task<(bool Success, AirlineHandlingContractDto? Result, string? Error)> UpdateAsync(
        int id, UpdateAirlineHandlingContractRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await client.PutAsJsonAsync($"/api/handling-contracts/{id}", request, ct);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<AirlineHandlingContractDto>(ct);
                return (true, dto, null);
            }
            var error = await response.ReadApiErrorAsync(ct);
            return (false, null, error);
        }
        catch (Exception ex) { return (false, null, ex.Message); }
    }

    public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var response = await client.DeleteAsync($"/api/handling-contracts/{id}", ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<IReadOnlyList<CompanyDto>> GetHandlingAgentCompaniesAsync(CancellationToken ct = default)
    {
        try
        {
            var all = await client.GetFromJsonAsync<List<CompanyDto>>("/api/companies", ct) ?? [];
            return [.. all.Where(c => c.Type == "HandlingAgent")];
        }
        catch { return []; }
    }
}
