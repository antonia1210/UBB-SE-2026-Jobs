namespace UBB_SE_2026_Jobs.Library.Services.CooldownService;

public interface ICooldownService
{
    Task<bool> IsOnCooldownAsync(int userId, int jobId, DateTime utcNow, CancellationToken cancellationToken = default);
}
