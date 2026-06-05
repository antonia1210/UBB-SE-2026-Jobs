using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using UBB_SE_2026_Jobs.App.Dtos.TI;

namespace UBB_SE_2026_Jobs.App.Services.TI;

public interface ITiInterviewSessionService
{
    Task<List<TiInterviewSessionDto>> GetScheduledAsync(int recruiterId);
    Task SetInterviewDecisionAsync(int sessionId, string decision);
}

public class TiInterviewSessionService : ITiInterviewSessionService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    private readonly HttpClient http;

    public TiInterviewSessionService(HttpClient http) => this.http = http;

    public async Task<List<TiInterviewSessionDto>> GetScheduledAsync(int recruiterId)
    {
        var response = await http.GetAsync($"api/interviewsessions/scheduled?recruiterId={recruiterId}");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiInterviewSessionDto>>(JsonOptions) ?? new();
    }

    public async Task SetInterviewDecisionAsync(int sessionId, string decision)
    {
        var response = await http.PostAsync($"api/interviewsessions/{sessionId}?decision={decision}", null);
        response.EnsureSuccessStatusCode();
    }
}