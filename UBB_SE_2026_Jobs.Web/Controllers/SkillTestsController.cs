using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Services.SkillTests;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize(Roles = "Candidate")]
public class SkillTestsController : Controller
{
    private readonly ISkillTestService skillTestService;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public SkillTestsController(ISkillTestService skillTestService)
    {
        this.skillTestService = skillTestService;
    }

    /// <summary>
    /// Displays all completed test results for the current user.
    /// The view receives IReadOnlyList&lt;SkillTestViewDto&gt; instead of
    /// IReadOnlyList&lt;SkillTest&gt;. Update the view's @model directive accordingly
    /// (see note below) — no other view change is needed because the property
    /// names (Name, Score, AchievedDate) are identical.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var tests = await skillTestService.GetTestsForUserAsync(CurrentUserId);
        return View(tests);
    }

    // Retake action removed: once a test is completed it cannot be retaken.
    // The TestAttempts pipeline (TestAttemptsController) handles new attempts.
}

/*
 * VIEW UPDATE REQUIRED (minimal — one line):
 *
 * In Views/SkillTests/Index.cshtml change:
 *   @model IReadOnlyList<UBB_SE_2026_Jobs.Library.Domain.SkillTest>
 * to:
 *   @model IReadOnlyList<UBB_SE_2026_Jobs.Library.DTOs.SkillTestViewDto>
 *
 * All rendered properties (Name, Score, AchievedDate) have the same names,
 * so the rest of the view template is unchanged.
 * If you had a "Retake" form/button, remove it — that action no longer exists.
 */