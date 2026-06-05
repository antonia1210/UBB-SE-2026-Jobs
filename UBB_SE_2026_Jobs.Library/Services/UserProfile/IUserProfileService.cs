using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.UserProfileService;

public interface IUserProfileService
{
    Task<User?> GetProfileAsync(int userId, CancellationToken cancellationToken = default);

    Task<bool> IsProfileAvailableAsync(int userId, CancellationToken cancellationToken = default);

    Task UpdateAccountStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default);

    Task UpdateProfilePicturePathAsync(int userId, string newPath, CancellationToken cancellationToken = default);

    Task RemoveProfilePicturePathAsync(int userId, CancellationToken cancellationToken = default);

    Task SaveAsync(int userId, User user, CancellationToken cancellationToken = default);

    Task<User> UploadCvAsync(int userId, Stream stream, string fileName, CancellationToken cancellationToken = default);

    Task<int> RecalculateLevelAsync(User user, CancellationToken cancellationToken = default);

}