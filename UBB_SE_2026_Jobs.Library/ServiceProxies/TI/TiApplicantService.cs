using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using UBB_SE_2026_Jobs.Library.DTOs.TI;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.TI;

public interface ITiApplicantService
{
    Task<List<TiMatchSummaryDto>> GetByJobAsync(int jobId);
    Task<TiApplicantDto?> CreateAsync(TiApplicantDto dto);
    Task<bool> HasUserAppliedAsync(int jobId, int userId);
}

public class TiApplicantService : ITiApplicantService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    private readonly HttpClient http;

    public TiApplicantService(HttpClient http) => this.http = http;

    public async Task<List<TiMatchSummaryDto>> GetByJobAsync(int jobId)
    {
        var response = await http.GetAsync($"api/matches?jobId={jobId}");
        if (!response.IsSuccessStatusCode) return new();
        var matches = await response.Content.ReadFromJsonAsync<List<MatchApiResponse>>(JsonOptions) ?? new();
        return matches.Select(match => new TiMatchSummaryDto
        {
            MatchId = match.MatchId,
            UserId = match.User?.UserId ?? 0,
            CandidateName = $"{match.User?.FirstName} {match.User?.LastName}".Trim() is { Length: > 0 } candidateName
                ? candidateName
                : $"User {match.User?.UserId}",
            Email = match.User?.Email ?? string.Empty,
            Status = match.Status,
            AppliedAt = match.Timestamp,
            FeedbackMessage = match.FeedbackMessage ?? string.Empty,
        }).ToList();
    }

    public async Task<TiApplicantDto?> CreateAsync(TiApplicantDto dto)
    {
        var response = await http.PostAsJsonAsync("api/applicants", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TiApplicantDto>();
    }

    public async Task<bool> HasUserAppliedAsync(int jobId, int userId)
    {
        var matches = await GetByJobAsync(jobId);
        return matches.Any(match => match.UserId == userId);
    }

    private sealed class MatchApiResponse
    {
        public int MatchId { get; set; }
        public MatchUserResponse? User { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? FeedbackMessage { get; set; }
    }

    private sealed class MatchUserResponse
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }
}
