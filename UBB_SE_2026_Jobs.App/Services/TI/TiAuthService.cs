using System.Net.Http.Json;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.App.Services.TI;

public interface ITiAuthService
{
    /// <summary>
    /// Retrieves the list of companies from the T&amp;I API, used to populate the
    /// recruiter company picker. Returns an empty list if the call fails.
    /// </summary>
    Task<List<TiCompanyDto>> GetCompaniesAsync();

    Task<AuthResponseDto?> LoginAsync(string email, string password);

    /// <summary>
    /// Registers a user with the T&amp;I API. For recruiters this creates the
    /// Recruiter record tied to <paramref name="companyId"/>.
    /// </summary>
    Task<bool> RegisterAsync(string name, string email, string password, string role, int? companyId = null);
}

public class TiAuthService : ITiAuthService
{
    private readonly HttpClient http;

    public TiAuthService(HttpClient http) => this.http = http;

    public async Task<List<TiCompanyDto>> GetCompaniesAsync()
    {
        try
        {
            var companies = await http.GetFromJsonAsync<List<TiCompanyDto>>("api/TestsCompanies");
            return companies ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<AuthResponseDto?> LoginAsync(string email, string password)
    {
        var payload = new { Email = email, Password = password };
        var response = await http.PostAsJsonAsync("api/tests-auth/login", payload);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    public async Task<bool> RegisterAsync(string name, string email, string password, string role, int? companyId = null)
    {
        var payload = new
        {
            Name = name,
            Email = email,
            Password = password,
            Role = role,
            CompanyId = companyId,
        };

        var response = await http.PostAsJsonAsync("api/tests-auth/register", payload);
        return response.IsSuccessStatusCode;
    }
}
