using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services.ChatService;
using UBB_SE_2026_Jobs.Library.Services.FileStorage;
using UBB_SE_2026_Jobs.Web.Configuration;
using UBB_SE_2026_Jobs.Web.Infrastructure;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize]
public class ChatController : Controller
{
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png" };

    private readonly IChatService chat;
    private readonly ILocalFileStorageService fileStorage;
    private readonly ApiConfiguration apiConfiguration;

    public ChatController(IChatService chat, ILocalFileStorageService fileStorage, ApiConfiguration apiConfiguration)
    {
        this.chat = chat;
        this.fileStorage = fileStorage;
        this.apiConfiguration = apiConfiguration;
    }

    public async Task<IActionResult> Index(string? tab, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var chats = await chat.GetChatsForUserAsync(userId, cancellationToken);
        ViewBag.IsCompanyMode = IsCompanyMode();
        ViewBag.ActiveTab = (tab == "companies") ? "companies" : "users";
        return View(chats);
    }

    public async Task<IActionResult> Show(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var messages = await chat.GetMessagesAsync(id, userId, null, cancellationToken);
        await chat.MarkMessagesAsReadAsync(id, userId, cancellationToken);
        ViewBag.ChatId = id;
        ViewBag.CurrentUserId = userId;
        ViewBag.ApiBase = apiConfiguration.BaseUrl.TrimEnd('/') + "/api/files";
        return View(messages);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int id, string content, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (!string.IsNullOrWhiteSpace(content))
            await chat.SendMessageAsync(id, content, userId, MessageType.Text, null, cancellationToken);
        return RedirectToAction(nameof(Show), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SendAttachment(int id, IFormFile attachment, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (attachment is null || attachment.Length == 0)
        {
            TempData["ChatError"] = "No file selected.";
            return RedirectToAction(nameof(Show), new { id });
        }

        try
        {
            await using var stream = attachment.OpenReadStream();
            var path = await fileStorage.SaveFileAsync(stream, attachment.FileName, cancellationToken);
            var attachementExtension = Path.GetExtension(attachment.FileName);
            var type = ImageExtensions.Contains(attachementExtension) ? MessageType.Image : MessageType.File;
            await chat.SendStoredAttachmentAsync(id, path, attachment.FileName, userId, type, null, cancellationToken);
        }
        catch (Exception exception)
        {
            TempData["ChatError"] = exception.Message;
        }

        return RedirectToAction(nameof(Show), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> SearchUsers(string q, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Json(Array.Empty<object>());

        var currentUserId = GetUserId();

        if (IsCompanyMode())
        {
            var companyId = GetCompanyId();
            var recruiters = await chat.SearchRecruitersByCompanyAsync(companyId, q, cancellationToken);
            return Json(recruiters
                .Where(u => u.UserId != currentUserId)
                .Select(u => new { id = u.UserId, name = $"{u.FirstName} {u.LastName}".Trim() }));
        }

        var users = await chat.SearchUsersAsync(q, cancellationToken);
        return Json(users
            .Where(u => u.UserId != currentUserId)
            .Select(u => new { id = u.UserId, name = $"{u.FirstName} {u.LastName}".Trim() }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> StartChat(int targetUserId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (targetUserId == userId)
            return RedirectToAction(nameof(Index));
        var newChat = await chat.FindOrCreateUserChatAsync(userId, targetUserId, cancellationToken);
        if (newChat is null)
            return RedirectToAction(nameof(Index));
        return RedirectToAction(nameof(Show), new { id = newChat.ChatId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await chat.DeleteChatAsync(id, userId, null, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Block(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await chat.BlockChatAsync(id, userId, null, cancellationToken);
        return RedirectToAction(nameof(Show), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Unblock(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await chat.UnblockChatAsync(id, userId, null, cancellationToken);
        return RedirectToAction(nameof(Show), new { id });
    }

    private int GetUserId()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(idValue))
        {
            throw new InvalidOperationException("User session is missing a user id.");
        }

        return int.Parse(idValue, System.Globalization.CultureInfo.InvariantCulture);
    }

    private int GetCompanyId()
    {
        var companyIdValue = User.FindFirstValue("CompanyId");
        if (!string.IsNullOrWhiteSpace(companyIdValue) && int.TryParse(companyIdValue, System.Globalization.CultureInfo.InvariantCulture, out var companyId))
        {
            return companyId;
        }

        // Fallback for backwards compatibility
        return apiConfiguration.TemporaryCompanyId;
    }

    private bool IsCompanyMode()
        => string.Equals(HttpContext.Session.GetString(SessionKeys.Mode), AppModes.Company, StringComparison.OrdinalIgnoreCase);
}
