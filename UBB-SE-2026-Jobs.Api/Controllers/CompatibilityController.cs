using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services.CompatibilityService;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/compatibility")]
public class CompatibilityController : ControllerBase
{
    private readonly ICompatibilityService compatibility;

    public CompatibilityController(ICompatibilityService compatibility)
    {
        this.compatibility = compatibility;
    }

    [HttpGet("{userId}/all")]
    public async Task<IActionResult> GetAll(int userId, CancellationToken cancellationToken)
        => Ok(await compatibility.CalculateAllAsync(userId, cancellationToken));

    [HttpGet("{userId}/role/{role}")]
    public async Task<IActionResult> GetForRole(int userId, JobRole role, CancellationToken cancellationToken)
        => Ok(await compatibility.CalculateForRoleAsync(userId, role, cancellationToken));
}
