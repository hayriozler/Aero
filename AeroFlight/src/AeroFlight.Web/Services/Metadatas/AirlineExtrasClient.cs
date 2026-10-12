using AeroFlight.Contracts.Metadatas;
using System.Net;
using System.Net.Http.Headers;

namespace AeroFlight.Web.Services.Metadatas;

public interface IAirlineExtrasClient
{
    Task<IReadOnlyList<AirlineContactResponse>> GetContactsAsync(string airlineCode, CancellationToken ct = default);
    Task<AirlineContactResponse> AddContactAsync(string airlineCode, AirlineContactRequest request, CancellationToken ct = default);
    Task<AirlineContactResponse?> UpdateContactAsync(string airlineCode, int id, AirlineContactRequest request, CancellationToken ct = default);
    Task<bool> DeleteContactAsync(string airlineCode, int id, CancellationToken ct = default);

    Task<IReadOnlyList<AirlineLogoResponse>> GetLogosAsync(string airlineCode, CancellationToken ct = default);
    Task<(byte[] Data, string ContentType)?> GetLogoImageAsync(string airlineCode, int id, CancellationToken ct = default);
    Task<AirlineLogoResponse> AddLogoAsync(string airlineCode, string name, byte[] data, string fileName, string contentType, CancellationToken ct = default);
    Task<AirlineLogoResponse?> RenameLogoAsync(string airlineCode, int id, string name, CancellationToken ct = default);
    Task<bool> DeleteLogoAsync(string airlineCode, int id, CancellationToken ct = default);
}

public sealed class AirlineExtrasClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IAirlineExtrasClient
{
    private static string ContactsRoute(string airlineCode) => $"/api/metadata/airlines/{Uri.EscapeDataString(airlineCode)}/contacts";
    private static string LogosRoute(string airlineCode) => $"/api/metadata/airlines/{Uri.EscapeDataString(airlineCode)}/logos";

    public Task<IReadOnlyList<AirlineContactResponse>> GetContactsAsync(string airlineCode, CancellationToken ct = default)
        => GetListAsync<AirlineContactResponse>(ContactsRoute(airlineCode), ct);

    public Task<AirlineContactResponse> AddContactAsync(string airlineCode, AirlineContactRequest request, CancellationToken ct = default)
        => PostForResultAsync<AirlineContactResponse>(ContactsRoute(airlineCode), request, ct);

    public Task<AirlineContactResponse?> UpdateContactAsync(string airlineCode, int id, AirlineContactRequest request, CancellationToken ct = default)
        => PutForOptionalResultAsync<AirlineContactResponse>($"{ContactsRoute(airlineCode)}/{id}", request, ct);

    public Task<bool> DeleteContactAsync(string airlineCode, int id, CancellationToken ct = default)
        => DeleteAsync($"{ContactsRoute(airlineCode)}/{id}", ct);

    public Task<IReadOnlyList<AirlineLogoResponse>> GetLogosAsync(string airlineCode, CancellationToken ct = default)
        => GetListAsync<AirlineLogoResponse>(LogosRoute(airlineCode), ct);

    public async Task<(byte[] Data, string ContentType)?> GetLogoImageAsync(string airlineCode, int id, CancellationToken ct = default)
    {
        using var request = CreateRequest(HttpMethod.Get, $"{LogosRoute(airlineCode)}/{id}");
        using var response = await Http.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureSuccessAsync(response, ct);
        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        return (bytes, response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream");
    }

    public async Task<AirlineLogoResponse> AddLogoAsync(string airlineCode, string name, byte[] data, string fileName, string contentType, CancellationToken ct = default)
    {
        using var request = CreateRequest(HttpMethod.Post, LogosRoute(airlineCode));
        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(data);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        using var stringContent = new StringContent(name);
        content.Add(stringContent, "name");
        request.Content = content;
        using var response = await Http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<AirlineLogoResponse>>(JsonOptions, ct);
        return envelope!.Data!;
    }

    public Task<AirlineLogoResponse?> RenameLogoAsync(string airlineCode, int id, string name, CancellationToken ct = default)
        => PutForOptionalResultAsync<AirlineLogoResponse>($"{LogosRoute(airlineCode)}/{id}", new AirlineLogoRenameRequest(name), ct);

    public Task<bool> DeleteLogoAsync(string airlineCode, int id, CancellationToken ct = default)
        => DeleteAsync($"{LogosRoute(airlineCode)}/{id}", ct);
}
