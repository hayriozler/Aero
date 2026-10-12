using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.WebApp.Services;

public sealed class ContainerApiClient(HttpClient http)
{
    // ── Containers (per flight) ───────────────────────────────────────────

    public async Task<IReadOnlyList<ContainerDto>> GetContainersByFlightAsync(
        int flightId, CancellationToken ct = default)
    {
        try
        {
            return await http.GetFromJsonAsync<List<ContainerDto>>($"/api/containers?flightId={flightId}", ct) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<(bool Success, ContainerDto? Data, string? Error)> CreateContainerAsync(
        CreateContainerRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/containers", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<ContainerDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, ContainerDto? Data, string? Error)> UpdateContainerAsync(
        int id, UpdateContainerRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/containers/{id}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<ContainerDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeleteContainerAsync(int id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/containers/{id}", ct);
        return response.IsSuccessStatusCode;
    }

    // ── Container Types ───────────────────────────────────────────────────

    public async Task<IReadOnlyList<ContainerTypeDto>> GetContainerTypesAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<ContainerTypeDto>>("/api/container-types", ct) ?? [];

    public async Task<(bool Success, ContainerTypeDto? Data, string? Error)> CreateContainerTypeAsync(
        CreateContainerTypeRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/container-types", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<ContainerTypeDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, ContainerTypeDto? Data, string? Error)> UpdateContainerTypeAsync(
        string code, UpdateContainerTypeRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/container-types/{code}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<ContainerTypeDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeleteContainerTypeAsync(string code, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/container-types/{code}", ct);
        return response.IsSuccessStatusCode;
    }

    // ── Container Classes ─────────────────────────────────────────────────

    public async Task<IReadOnlyList<ContainerClassDto>> GetContainerClassesAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<ContainerClassDto>>("/api/container-classes", ct) ?? [];

    public async Task<(bool Success, ContainerClassDto? Data, string? Error)> CreateContainerClassAsync(
        CreateContainerClassRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/container-classes", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<ContainerClassDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, ContainerClassDto? Data, string? Error)> UpdateContainerClassAsync(
        string typeCode, UpdateContainerClassRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/container-classes/{typeCode}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<ContainerClassDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeleteContainerClassAsync(string typeCode, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/container-classes/{typeCode}", ct);
        return response.IsSuccessStatusCode;
    }

    // ── Airline Class Maps ────────────────────────────────────────────────

    public async Task<IReadOnlyList<AirlineClassMapDto>> GetAirlineClassMapsAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<AirlineClassMapDto>>("/api/airline-class-maps", ct) ?? [];

    public async Task<(bool Success, AirlineClassMapDto? Data, string? Error)> CreateAirlineClassMapAsync(
        CreateAirlineClassMapRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/airline-class-maps", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<AirlineClassMapDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, AirlineClassMapDto? Data, string? Error)> UpdateAirlineClassMapAsync(
        string airlineCode, char sourceClass, UpdateAirlineClassMapRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/airline-class-maps/{airlineCode}/{sourceClass}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<AirlineClassMapDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeleteAirlineClassMapAsync(string airlineCode, char sourceClass, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/airline-class-maps/{airlineCode}/{sourceClass}", ct);
        return response.IsSuccessStatusCode;
    }

    // ── Resource Status Maps ──────────────────────────────────────────────

    public async Task<IReadOnlyList<ResourceStatusMapDto>> GetResourceStatusMapsAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<ResourceStatusMapDto>>("/api/resource-status-maps", ct) ?? [];

    public async Task<(bool Success, ResourceStatusMapDto? Data, string? Error)> CreateResourceStatusMapAsync(
        CreateResourceStatusMapRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/resource-status-maps", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<ResourceStatusMapDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, ResourceStatusMapDto? Data, string? Error)> UpdateResourceStatusMapAsync(
        string sourceName, string sourceStatus, UpdateResourceStatusMapRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/resource-status-maps/{sourceName}/{sourceStatus}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<ResourceStatusMapDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeleteResourceStatusMapAsync(string sourceName, string sourceStatus, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/resource-status-maps/{sourceName}/{sourceStatus}", ct);
        return response.IsSuccessStatusCode;
    }
}
