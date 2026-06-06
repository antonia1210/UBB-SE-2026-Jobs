using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.DTOs.Web;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public class InterviewSessionsApiClient
{
    private readonly HttpClient http;
    private const string SessionsPath = "api/interviewsessions";
    private const string BookingsPath = "api/bookings";

    public InterviewSessionsApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<InterviewSessionDto>> GetScheduledAsync(int recruiterId)
    {
        return await this.http.GetFromJsonAsync<List<InterviewSessionDto>>(
            $"{SessionsPath}/scheduled?recruiterId={recruiterId}") ?? new List<InterviewSessionDto>();
    }

    public async Task<List<InterviewSessionDto>> GetByStatusAsync(string status)
    {
        return await this.http.GetFromJsonAsync<List<InterviewSessionDto>>(
            $"{SessionsPath}/status/{status}") ?? new List<InterviewSessionDto>();
    }

    public async Task<List<InterviewSessionDto>> GetBookedByCandidate(int candidateId)
    {
        return await this.http.GetFromJsonAsync<List<InterviewSessionDto>>(
            $"{SessionsPath}/scheduled/{candidateId}") ?? new List<InterviewSessionDto>();
    }

    public async Task ConfirmBookingAsync(int slotId, int candidateId, int jobId)
    {
        var response = await this.http.PostAsJsonAsync(
            $"{BookingsPath}/{slotId}/confirm?jobId={jobId}", candidateId);
        response.EnsureSuccessStatusCode();
    }

    public async Task SetInterviewDecision(int sessionId, string decision)
    {
        var response = await this.http.PostAsync($"{SessionsPath}/{sessionId}?decision={decision}", null);
        response.EnsureSuccessStatusCode();
    }
}
