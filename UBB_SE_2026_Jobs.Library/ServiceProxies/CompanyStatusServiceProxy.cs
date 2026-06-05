using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.CompanyStatusService;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies;

public class CompanyStatusServiceProxy : ICompanyStatusService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient http;

    public CompanyStatusServiceProxy(HttpClient http)
    {
        this.http = http;
    }

    public async Task<IReadOnlyList<UserApplicationResult>> GetApplicantsForCompanyAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        return await http.GetFromJsonAsync<List<UserApplicationResult>>(
                   $"api/company-status/companies/{companyId}/applicants",
                   JsonOptions,
                   cancellationToken)
               ?? new List<UserApplicationResult>();
    }

    public async Task<UserApplicationResult?> GetApplicantByMatchIdAsync(
        int companyId,
        int matchId,
        CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync(
            $"api/company-status/companies/{companyId}/applicants/{matchId}",
            cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserApplicationResult>(JsonOptions, cancellationToken);
    }
}
