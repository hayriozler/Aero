using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Web.Services;
using System.Net;
using System.Net.Http.Headers;

namespace Aero.Flights.Web.Services.Metadatas;

public interface IAirportImagesClient
{
    Task<IReadOnlyList<AirportImageResponse>> GetImagesAsync(string airportCode, CancellationToken ct = default);
    Task<(byte[] Data, string ContentType)?> GetImageAsync(string airportCode, int id, CancellationToken ct = default);
    Task<AirportImageResponse> UploadImageAsync(string airportCode, byte[] data, string fileName, string contentType, int? displayOrder, CancellationToken ct = default);
    Task<bool> DeleteImageAsync(string airportCode, int id, CancellationToken ct = default);
}

public sealed class AirportImagesClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IAirportImagesClient
{
    private static string ImagesRoute(string airportCode) => $"/api/metadata/airports/{Uri.EscapeDataString(airportCode)}/images";

    public Task<IReadOnlyList<AirportImageResponse>> GetImagesAsync(string airportCode, CancellationToken ct = default)
        => GetListAsync<AirportImageResponse>(ImagesRoute(airportCode), ct);

    public async Task<(byte[] Data, string ContentType)?> GetImageAsync(string airportCode, int id, CancellationToken ct = default)
    {
        using var request = CreateRequest(HttpMethod.Get, $"{ImagesRoute(airportCode)}/{id}");
        using var response = await Http.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureSuccessAsync(response, ct);
        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        return (bytes, response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream");
    }

    public async Task<AirportImageResponse> UploadImageAsync(string airportCode, byte[] data, string fileName, string contentType, int? displayOrder, CancellationToken ct = default)
    {
        using var request = CreateRequest(HttpMethod.Post, ImagesRoute(airportCode));
        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(data);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        if (displayOrder is not null)
            content.Add(new StringContent(displayOrder.Value.ToString()), "displayOrder");
        request.Content = content;

        using var response = await Http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<AirportImageResponse>>(JsonOptions, ct);
        return envelope!.Data!;
    }

    public Task<bool> DeleteImageAsync(string airportCode, int id, CancellationToken ct = default)
        => DeleteAsync($"{ImagesRoute(airportCode)}/{id}", ct);
}
