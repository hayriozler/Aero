using Aero.Flights.Contracts.Metadatas;

namespace Aero.Flights.Web.Services.Metadatas;

public interface ICompanyExtrasClient
{
    Task<IReadOnlyList<CompanyContactResponse>> GetContactsAsync(string companyCode, CancellationToken ct = default);
    Task<CompanyContactResponse> AddContactAsync(string companyCode, CompanyContactRequest request, CancellationToken ct = default);
    Task<CompanyContactResponse?> UpdateContactAsync(string companyCode, int id, CompanyContactRequest request, CancellationToken ct = default);
    Task<bool> DeleteContactAsync(string companyCode, int id, CancellationToken ct = default);
}

public sealed class CompanyExtrasClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), ICompanyExtrasClient
{
    private static string ContactsRoute(string companyCode) => $"/api/metadata/companies/{Uri.EscapeDataString(companyCode)}/contacts";

    public Task<IReadOnlyList<CompanyContactResponse>> GetContactsAsync(string companyCode, CancellationToken ct = default)
        => GetListAsync<CompanyContactResponse>(ContactsRoute(companyCode), ct);

    public Task<CompanyContactResponse> AddContactAsync(string companyCode, CompanyContactRequest request, CancellationToken ct = default)
        => PostForResultAsync<CompanyContactResponse>(ContactsRoute(companyCode), request, ct);

    public Task<CompanyContactResponse?> UpdateContactAsync(string companyCode, int id, CompanyContactRequest request, CancellationToken ct = default)
        => PutForOptionalResultAsync<CompanyContactResponse>($"{ContactsRoute(companyCode)}/{id}", request, ct);

    public Task<bool> DeleteContactAsync(string companyCode, int id, CancellationToken ct = default)
        => DeleteAsync($"{ContactsRoute(companyCode)}/{id}", ct);
}
