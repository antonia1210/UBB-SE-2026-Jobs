using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.DTOs;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers.TestsAndInterviews
{
    /// <summary>
    /// Handles authentication endpoints.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// Validates credentials and returns a JWT.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            AuthResponseDto? result = await this.authService.LoginAsync(dto);

            if (result == null)
            {
                return this.Unauthorized("Invalid email or password.");
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Registers a new user and returns a JWT.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            AuthResponseDto? result = await this.authService.RegisterAsync(dto);

            if (result == null)
            {
                return this.Conflict("A user with this email already exists.");
            }

            return this.Ok(result);
        }
    }
}