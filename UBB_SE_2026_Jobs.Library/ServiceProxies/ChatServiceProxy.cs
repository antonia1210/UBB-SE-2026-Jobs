using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services.ChatService;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies;

public class ChatServiceProxy : IChatService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient http;

    public ChatServiceProxy(HttpClient http) => this.http = http;

    private static string WithCompany(string url, int? companyId)
        => companyId.HasValue ? url + (url.Contains('?') ? "&" : "?") + "companyId=" + companyId.Value : url;

    public async Task<IReadOnlyList<Chat>> GetChatsForUserAsync(int userId, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<Chat>>($"api/chats?userId={userId}&callerId={userId}", JsonOptions, cancellationToken)
           ?? new List<Chat>();

    public async Task<IReadOnlyList<Chat>> GetChatsForCompanyAsync(int companyId, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<Chat>>($"api/chats?companyId={companyId}&callerId={companyId}", JsonOptions, cancellationToken)
           ?? new List<Chat>();

    public async Task<IReadOnlyList<Message>> GetMessagesAsync(int chatId, int callerId, int? companyId = null, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<Message>>(WithCompany($"api/chats/{chatId}/messages?callerId={callerId}", companyId), JsonOptions, cancellationToken)
           ?? new List<Message>();

    public async Task SendMessageAsync(int chatId, string content, int senderId, MessageType type, int? companyId = null, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync(WithCompany($"api/chats/{chatId}/messages", companyId),
            new { senderId, content, type }, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task MarkMessagesAsReadAsync(int chatId, int readerId, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsync($"api/chats/{chatId}/messages/read?readerId={readerId}", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task BlockChatAsync(int chatId, int blockerId, int? companyId = null, CancellationToken cancellationToken = default)
    {
        var response = await http.PatchAsJsonAsync(WithCompany($"api/chats/{chatId}/block", companyId), new { callerId = blockerId }, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnblockChatAsync(int chatId, int unblockerId, int? companyId = null, CancellationToken cancellationToken = default)
    {
        var response = await http.PatchAsJsonAsync(WithCompany($"api/chats/{chatId}/unblock", companyId), new { callerId = unblockerId }, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteChatAsync(int chatId, int callerId, int? companyId = null, CancellationToken cancellationToken = default)
    {
        var response = await http.DeleteAsync(WithCompany($"api/chats/{chatId}?callerId={callerId}", companyId), cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Chat?> FindOrCreateUserChatAsync(int userId, int secondUserId, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("api/chats", new { userId, secondUserId, company = (object?)null, job = (object?)null }, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Chat>(JsonOptions, cancellationToken: cancellationToken);
    }

    public async Task<Chat?> FindOrCreateUserCompanyChatAsync(int userId, Company company, Job? job = null, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("api/chats", new { userId, secondUserId = (int?)null, company, job }, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Chat>(JsonOptions, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<User>> SearchUsersAsync(string query, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<User>>($"api/chats/search/users?userQuery={Uri.EscapeDataString(query)}", JsonOptions, cancellationToken)
           ?? new List<User>();

    public async Task<IReadOnlyList<Company>> SearchCompaniesAsync(string query, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<Company>>($"api/chats/search/companies?companyQuery={Uri.EscapeDataString(query)}", JsonOptions, cancellationToken)
           ?? new List<Company>();

    public async Task<IReadOnlyList<User>> SearchRecruitersByCompanyAsync(int companyId, string query, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<User>>($"api/chats/search/recruiters?companyId={companyId}&query={Uri.EscapeDataString(query)}", JsonOptions, cancellationToken)
           ?? new List<User>();

    public async Task SendStoredAttachmentAsync(int chatId, string storedPath, string originalFileName, int senderId, MessageType type, int? companyId = null, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync(WithCompany($"api/chats/{chatId}/attachments", companyId),
            new { senderId, storedPath, originalFileName, type }, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Stream> OpenMessageAttachmentAsync(string attachmentPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(attachmentPath))
            throw new ArgumentException("Attachment path cannot be empty.", nameof(attachmentPath));

        var fileName = Uri.EscapeDataString(Path.GetFileName(attachmentPath));
        var response = await http.GetAsync($"api/files/{fileName}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var memory = new MemoryStream();
        await response.Content.CopyToAsync(memory, cancellationToken).ConfigureAwait(false);
        memory.Position = 0;
        return memory;
    }
}
