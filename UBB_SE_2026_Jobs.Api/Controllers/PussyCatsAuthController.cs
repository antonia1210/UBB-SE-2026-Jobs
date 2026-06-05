using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.Users;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth")]
public class PussyCatsAuthController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IConfiguration configuration;

    public PussyCatsAuthController(IUserService userService, IConfiguration configuration)
    {
        this.userService = userService;
        this.configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var user = await userService.GetByEmailAsync(loginRequest.Email, cancellationToken);
        if (user is null)
            return Unauthorized();

        var passwordHasher = new PasswordHasher<User>();
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
            return Unauthorized();

        return Ok(ToAuthResponse(user));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        if (await userService.ExistsWithEmailAsync(registerRequest.Email, cancellationToken))
            return Conflict("Email already registered.");

        var newUser = new User
        {
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
        };

        var passwordHasher = new PasswordHasher<User>();
        newUser.PasswordHash = passwordHasher.HashPassword(newUser, registerRequest.Password);

        var savedUser = await userService.AddAsync(newUser, cancellationToken);
        return Ok(ToAuthResponse(savedUser));
    }

    private AuthResponse ToAuthResponse(User user)
    {
        return new AuthResponse(
            user.UserId,
            user.Email,
            user.FirstName,
            user.LastName,
            GenerateJwt(user));
    }

    private string GenerateJwt(User user)
    {
        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("Missing JWT signing key configuration.");
        }

        var issuer = configuration["Jwt:Issuer"] ?? "UBB_SE_2026_Jobs.Api";
        var audience = configuration["Jwt:Audience"] ?? "PussyCats.Clients";
        var expiresMinutes = configuration.GetValue("Jwt:ExpiresMinutes", 120);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Email", user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var jwtToken = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
}