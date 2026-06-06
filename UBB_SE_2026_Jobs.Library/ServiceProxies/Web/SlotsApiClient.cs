using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.DTOs.Web;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public class SlotsApiClient
{
    private readonly HttpClient http;
    private const string ApiPath = "api/slots";

    public SlotsApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<SlotDto>> GetAvailableAsync(DateTime date)
    {
        string formattedDate = date.ToString("yyyy-MM-dd");
        return await this.http.GetFromJsonAsync<List<SlotDto>>(
            $"{ApiPath}/available?date={formattedDate}") ?? new List<SlotDto>();
    }

    public async Task<List<SlotDto>> GetAllByRecruiterAsync(int recruiterId)
    {
        return await this.http.GetFromJsonAsync<List<SlotDto>>(
            $"{ApiPath}/recruiter/{recruiterId}") ?? new List<SlotDto>();
    }

    public async Task<List<CompanyDto>> GetCompaniesForRecruiterAsync(int recruiterId)
    {
        return await this.http.GetFromJsonAsync<List<CompanyDto>>($"api/TestsCompanies/byrecruiter/{recruiterId}") ?? new List<CompanyDto>();
    }

    public async Task<List<SlotDto>> GetAvailableSlotsForCompany(int companyId, DateTime date)
    {
        string formattedDate = date.ToString("yyyy-MM-dd");
        return await this.http.GetFromJsonAsync<List<SlotDto>>(
            $"{ApiPath}/company/{companyId}?date={formattedDate}") ?? new List<SlotDto>();
    }

    public async Task AddRecruiterSlotAsync(SlotDto baseSlot, int duration)
    {
        var payload = new { BaseSlot = baseSlot, Duration = duration };
        HttpResponseMessage response = await this.http.PostAsJsonAsync($"{ApiPath}/recruiter/create", payload);
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(message) ? "Slot overlaps with an existing appointment." : message);
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateRecruiterSlotAsync(SlotDto initialSlot, DateTime startime, int duration)
    {
        var payload = new { InitialSlot = initialSlot, StartTime = startime, Duration = duration };
        HttpResponseMessage response = await this.http.PutAsJsonAsync($"{ApiPath}/recruiter/update", payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteRecruiterSlotAsync(int slotId)
    {
        HttpResponseMessage response = await this.http.DeleteAsync($"{ApiPath}/{slotId}");
        response.EnsureSuccessStatusCode();
    }
}
