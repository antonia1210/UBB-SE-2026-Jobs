using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Services.SkillTests;

namespace UBB_SE_2026_Jobs.Api.Controllers;

/// <summary>
/// Read-only API for completed test results, shaped for profile/badge display.
/// Previously served the SkillTests table directly; now backed by TestAttempt data.
/// Mutating endpoints (Add, UpdateScore, UpdateDate, Delete, Retake) have been
/// removed — test results are the responsibility of the TestAttempts pipeline.
/// </summary>
[Authorize]
[ApiController]
[Route("api/skill-tests")]
public class SkillTestsController : ControllerBase
{
    private readonly ISkillTestService skillTestService;

    public SkillTestsController(ISkillTestService skillTestService)
    {
        this.skillTestService = skillTestService;
    }

    /// <summary>
    /// Returns all completed test results for a user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByUserId(
        [FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        var results = await skillTestService.GetTestsForUserAsync(userId, cancellationToken);
        return Ok(results);
    }

    /// <summary>
    /// Returns a single completed test result by its attempt ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await skillTestService.GetSkillTestByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}