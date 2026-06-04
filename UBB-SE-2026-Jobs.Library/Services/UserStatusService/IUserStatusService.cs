using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Library.Services.UserStatusService;

public interface IUserStatusService
{
    Task<IReadOnlyList<ApplicationCardModel>> GetApplicationsForUserAsync(int userId, CancellationToken cancellationToken = default);
}
