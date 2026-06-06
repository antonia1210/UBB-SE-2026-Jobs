using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.DTOs.TI;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.TI;

public interface ITiEventsService
{
    Task<List<TiEventDto>> GetCurrentEventsAsync(int companyId);
    Task<List<TiEventDto>> GetPastEventsAsync(int companyId);
    Task<TiEventDto?> CreateAsync(TiEventDto dto);
    Task UpdateAsync(int id, TiEventDto dto);
    Task DeleteAsync(int id);
    Task<List<TiCollaboratorDto>> GetCollaboratorsAsync(int eventId);
}

public class TiEventsService : ITiEventsService
{
    private readonly HttpClient http;

    public TiEventsService(HttpClient http) => this.http = http;

    public async Task<List<TiEventDto>> GetCurrentEventsAsync(int companyId)
    {
        var response = await http.GetAsync($"api/events/current/{companyId}");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiEventDto>>() ?? new();
    }

    public async Task<List<TiEventDto>> GetPastEventsAsync(int companyId)
    {
        var response = await http.GetAsync($"api/events/past/{companyId}");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiEventDto>>() ?? new();
    }

    public async Task<TiEventDto?> CreateAsync(TiEventDto dto)
    {
        var response = await http.PostAsJsonAsync("api/events", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TiEventDto>();
    }

    public async Task UpdateAsync(int id, TiEventDto dto)
    {
        var response = await http.PutAsJsonAsync($"api/events/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/events/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<TiCollaboratorDto>> GetCollaboratorsAsync(int eventId)
    {
        var response = await http.GetAsync($"api/collaborators/event/{eventId}");
        if (!response.IsSuccessStatusCode) return new();
        return await response.Content.ReadFromJsonAsync<List<TiCollaboratorDto>>() ?? new();
    }
}
