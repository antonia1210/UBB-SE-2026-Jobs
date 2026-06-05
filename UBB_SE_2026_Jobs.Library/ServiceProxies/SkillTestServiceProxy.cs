using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.SkillTests;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies;

/// <summary>
/// HTTP proxy for the skill-tests API, which now returns completed TestAttempt
/// projections rather than the removed SkillTest entity.
/// Mutating methods (Add, UpdateScore, UpdateDate, Remove, retake) are gone —
/// test results are owned by the TestAttempts pipeline.
/// </summary>
public class SkillTestServiceProxy : ISkillTestService
{
    private readonly HttpClient httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public SkillTestServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SkillTestViewDto>> GetTestsForUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await httpClient
            .GetFromJsonAsync<List<SkillTestViewDto>>(
                $"api/skill-tests?userId={userId}",
                JsonOptions,
                cancellationToken)
            .ConfigureAwait(false)
               ?? new List<SkillTestViewDto>();
    }

    /// <inheritdoc/>
    public async Task<SkillTestViewDto?> GetSkillTestByIdAsync(
        int skillTestId,
        CancellationToken cancellationToken = default)
    {
        return await httpClient
            .GetFromJsonAsync<SkillTestViewDto>(
                $"api/skill-tests/{skillTestId}",
                JsonOptions,
                cancellationToken)
            .ConfigureAwait(false);
    }
}