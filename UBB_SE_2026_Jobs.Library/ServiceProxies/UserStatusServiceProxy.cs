using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies;

public class UserStatusServiceProxy : IUserStatusService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient http;

    public UserStatusServiceProxy(HttpClient http) => this.http = http;

    public async Task<IReadOnlyList<ApplicationCardModel>> GetApplicationsForUserAsync(int userId, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<ApplicationCardModel>>(
               $"api/user-status/{userId}/applications", JsonOptions, cancellationToken)
           ?? new List<ApplicationCardModel>();
}
