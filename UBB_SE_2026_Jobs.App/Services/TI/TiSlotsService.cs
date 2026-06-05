using System.Net.Http.Json;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.App.Services.TI;

public interface ITiSlotsService
{
    Task<List<TiApplicationDto>> GetApplicationsAsync(int candidateId);
    Task<List<TiSlotDto>> GetAvailableAsync(DateTime date, int? companyId = null);
    Task<List<TiSlotDto>> GetByRecruiterAsync(int recruiterId, DateTime date);
    Task<List<TiSlotDto>> GetMyBookingsAsync(int candidateId);
    Task<bool> BookSlotAsync(int slotId, int candidateId, int jobId);
    Task<List<TiInterviewSessionDto>> GetScheduledSessionsAsync();
    Task<List<TiInterviewSessionDto>> GetSessionsByStatusAsync(string status);
    Task<List<TiInterviewSessionDto>> GetBookedInterviewsByCandidate(int candidateId);
    Task<int?> GetCompanyIdByJobAsync(int jobId);
}

public class TiSlotsService : ITiSlotsService
{
    private readonly HttpClient http;

    public TiSlotsService(HttpClient http) => this.http = http;

    public async Task<List<TiApplicationDto>> GetApplicationsAsync(int candidateId)
    {
        var response = await http.GetAsync($"api/user-status/{candidateId}/applications");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiApplicationDto>>() ?? new();
    }

    public async Task<List<TiSlotDto>> GetAvailableAsync(DateTime date, int? companyId = null)
    {
        if (companyId.HasValue)
        {
            var response = await http.GetAsync($"api/slots/company/{companyId.Value}?date={date:yyyy-MM-dd}");
            if (!response.IsSuccessStatusCode) return new();
            return await response.Content.ReadFromJsonAsync<List<TiSlotDto>>() ?? new();
        }
        else
        {
            var response = await http.GetAsync($"api/slots/available?date={date:yyyy-MM-dd}");
            if (!response.IsSuccessStatusCode) return new();
            return await response.Content.ReadFromJsonAsync<List<TiSlotDto>>() ?? new();
        }
    }

    public async Task<List<TiSlotDto>> GetByRecruiterAsync(int recruiterId, DateTime date)
    {
        var response = await http.GetAsync($"api/slots/recruiter/{recruiterId}?date={date:yyyy-MM-dd}");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiSlotDto>>() ?? new();
    }

    public async Task<List<TiSlotDto>> GetMyBookingsAsync(int candidateId)
    {
        var response = await http.GetAsync($"api/slots/candidate/{candidateId}");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiSlotDto>>() ?? new();
    }

    public async Task<bool> BookSlotAsync(int slotId, int candidateId, int jobId)
    {
        var response = await http.PostAsJsonAsync($"api/bookings/{slotId}/confirm?jobId={jobId}", candidateId);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<TiInterviewSessionDto>> GetScheduledSessionsAsync()
    {
        var response = await http.GetAsync("api/interviewsessions/scheduled");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiInterviewSessionDto>>() ?? new();
    }

    public async Task<List<TiInterviewSessionDto>> GetSessionsByStatusAsync(string status)
    {
        var response = await http.GetAsync($"api/interviewsessions/status/{status}");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiInterviewSessionDto>>() ?? new();
    }

    public async Task<List<TiInterviewSessionDto>> GetBookedInterviewsByCandidate(int candidateId)
    {
        var response = await http.GetAsync($"api/interviewsessions/scheduled/{candidateId}");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiInterviewSessionDto>>() ?? new();
    }

    public async Task<int?> GetCompanyIdByJobAsync(int jobId)
    {
        var response = await http.GetAsync($"api/jobs/{jobId}");
        if (!response.IsSuccessStatusCode) return null;
        var job = await response.Content.ReadFromJsonAsync<JobDto>();
        return job?.CompanyId;
    }
}
