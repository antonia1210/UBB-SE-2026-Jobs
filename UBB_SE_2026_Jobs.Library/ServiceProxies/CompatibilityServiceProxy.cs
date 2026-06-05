using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.CompatibilityService;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies;

public class CompatibilityServiceProxy : ICompatibilityService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient http;

    public CompatibilityServiceProxy(HttpClient http) => this.http = http;

    public async Task<IReadOnlyList<RoleResult>> CalculateAllAsync(int userId, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<RoleResult>>($"api/compatibility/{userId}/all", JsonOptions, cancellationToken)
           ?? new List<RoleResult>();

    public async Task<RoleResult> CalculateForRoleAsync(int userId, JobRole role, CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync($"api/compatibility/{userId}/role/{role}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RoleResult>(JsonOptions, cancellationToken: cancellationToken))!;
    }

    public IReadOnlyList<Suggestion> GetSuggestions(RoleResult result) => result.Suggestions;
}
