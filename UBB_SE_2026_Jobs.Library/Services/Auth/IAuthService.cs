using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Library.Services.Auth;

public interface IAuthService
{
    Task<AuthServiceResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<AuthServiceResult> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);
}
