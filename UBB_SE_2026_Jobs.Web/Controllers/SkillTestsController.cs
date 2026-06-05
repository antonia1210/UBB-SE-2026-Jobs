using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Services.SkillTests;

namespace UBB_SE_2026_Jobs.Web.Controllers
{
    [Authorize(Roles = "Candidate")]
    public class SkillTestsController : Controller
    {
        private readonly ISkillTestService skillTestService;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public SkillTestsController(ISkillTestService skillTestService)
        {
            this.skillTestService = skillTestService;
        }

        public async Task<IActionResult> Index()
        {
            var tests = await skillTestService.GetTestsForUserAsync(CurrentUserId);
            return View(tests);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Retake(int id)
        {
            bool isEligible = await skillTestService.CanRetakeTestAsync(id);
            if (!isEligible)
            {
                TempData["ErrorMessage"] = "This test is locked. You cannot retake it yet.";
                return RedirectToAction(nameof(Index));
            }

            int randomScore = Random.Shared.Next(0, 101);
            await skillTestService.SubmitRetakeAsync(id, randomScore);
            TempData["SuccessMessage"] = $"Test retaken! New score: {randomScore}%";
            return RedirectToAction(nameof(Index));
        }
    }
}
