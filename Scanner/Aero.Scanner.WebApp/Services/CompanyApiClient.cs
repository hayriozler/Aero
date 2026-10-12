using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.WebApp.Services;

public sealed class CompanyApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<CompanyDto>> GetAllAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("/api/companies", ct);
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<CompanyDto>>(ct) ?? [];
    }

    public async Task<IReadOnlyList<CompanyDto>> GetHandlingAgentCompaniesAsync(CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return [.. all.Where(c => c.Type == "HandlingAgent").OrderBy(c => c.Name)];
    }

    public async Task<(bool Success, CompanyDto? Data, string? Error)> CreateAsync(
        CreateCompanyRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/companies", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<CompanyDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, CompanyDto? Data, string? Error)> UpdateAsync(
        string code, UpdateCompanyRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/companies/{code}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<CompanyDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeactivateAsync(string code, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/companies/{code}", ct);
        return response.IsSuccessStatusCode;
    }
}
