using System.Net.Http.Json;
using AuthResponseDto = UBB_SE_2026_Jobs.Library.DTOs.AuthResponseDto;
using UBB_SE_2026_Jobs.Library.DTOs.Web;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public class TiAuthService : ITiAuthService
{
    private readonly HttpClient http;

    public TiAuthService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<AuthResponseDto?> LoginAsync(string email, string password)
    {
        var payload = new { Email = email, Password = password };
        HttpResponseMessage response =
            await this.http.PostAsJsonAsync("api/tests-auth/login", payload);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    public async Task<AuthResponseDto?> RegisterAsync(
        string name, string email, string password, string role, int? companyId = null)
    {
        var payload = new
        {
            Name = name,
            Email = email,
            Password = password,
            Role = role,
            CompanyId = companyId,
        };

        HttpResponseMessage response =
            await this.http.PostAsJsonAsync("api/tests-auth/register", payload);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    public async Task<List<CompanyDto>> GetCompaniesAsync()
    {
        try
        {
            var companies = await this.http.GetFromJsonAsync<List<CompanyDto>>("api/TestsCompanies");
            return companies ?? new List<CompanyDto>();
        }
        catch
        {
            return new List<CompanyDto>();
        }
    }
}
