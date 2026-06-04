using UBB_SE_2026_Jobs.Library.DTOs.Portal;

namespace UBB_SE_2026_Jobs.Library.Services.Portal
{
    /// <summary>
    /// Handles authentication operations.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Validates credentials and returns a JWT if valid.
        /// </summary>
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);

        /// <summary>
        /// Registers a new user and returns a JWT.
        /// </summary>
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    }
}