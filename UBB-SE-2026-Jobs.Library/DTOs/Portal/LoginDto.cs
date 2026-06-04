namespace UBB_SE_2026_Jobs.Library.DTOs.Portal
{
    /// <summary>
    /// Data transfer object for login requests.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the plain-text password.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}