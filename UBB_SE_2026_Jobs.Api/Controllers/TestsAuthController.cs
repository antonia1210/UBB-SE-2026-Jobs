using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers;

/// <summary>
/// Handles authentication endpoints.
/// </summary>
[ApiController]
[Route("api/tests-auth")]
public class TestsAuthController : ControllerBase
{
    private readonly ITestsAuthService authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestsAuthController"/> class.
    /// </summary>
    public TestsAuthController(ITestsAuthService authService)
    {
        this.authService = authService;
    }

    /// <summary>
    /// Validates credentials and returns a JWT.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        AuthResponseDto? result = await this.authService.LoginAsync(loginDto);

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
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        AuthResponseDto? result = await this.authService.RegisterAsync(registerDto);

        if (result == null)
        {
            return this.Conflict("A user with this email already exists.");
        }

        return this.Ok(result);
    }
}