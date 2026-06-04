using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.Services.CooldownService;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies;

public class CooldownServiceProxy : ICooldownService
{
    private readonly HttpClient http;

    public CooldownServiceProxy(HttpClient http) => this.http = http;

    public async Task<bool> IsOnCooldownAsync(int userId, int jobId, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var result = await http.GetFromJsonAsync<CooldownResponse>(
            $"api/cooldown/users/{userId}/jobs/{jobId}", cancellationToken);
        return result?.IsOnCooldown ?? false;
    }

    private record CooldownResponse(bool IsOnCooldown);
}
