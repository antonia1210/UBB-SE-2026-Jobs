using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.UserRecommendationService;
using System.Security.Claims;

namespace UBB_SE_2026_Jobs.Web.Controllers
{
    [Authorize(Roles = "Candidate")]
    public class JobBrowserController : Controller
    {
        private const string SessionKeyEmploymentTypes = "Filter_EmploymentTypes";
        private const string SessionKeyExperienceLevels = "Filter_ExperienceLevels";
        private const string SessionKeyLocation = "Filter_Location";

        private readonly IUserRecommendationService recommendationService;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public JobBrowserController(IUserRecommendationService recommendationService)
        {
            this.recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index()
        {
            var filters = LoadFiltersFromSession();
            ViewBag.AppliedFilters = filters;
            var jobCard = await recommendationService.GetNextCardAsync(CurrentUserId, filters)
                ?? await recommendationService.RecalculateTopCardIgnoringCooldownAsync(CurrentUserId, filters);
            return View(jobCard);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(JobRecommendationResult card)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            try
            {
                await recommendationService.ApplyLikeAsync(CurrentUserId, card);
            }
            catch (Exception exception)
            {
                TempData["ErrorMessage"] = exception.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Dismiss(JobRecommendationResult card)
        {
            await recommendationService.ApplyDismissAsync(CurrentUserId, card);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ResetFilters()
        {
            HttpContext.Session.Remove(SessionKeyEmploymentTypes);
            HttpContext.Session.Remove(SessionKeyExperienceLevels);
            HttpContext.Session.Remove(SessionKeyLocation);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ApplyFilters(IFormCollection form)
        {
            var empTypes = form["EmploymentTypes"]
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();
            HttpContext.Session.SetString(SessionKeyEmploymentTypes, string.Join(",", empTypes));

            var expLevels = form["ExperienceLevels"]
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();
            HttpContext.Session.SetString(SessionKeyExperienceLevels, string.Join(",", expLevels));

            HttpContext.Session.SetString(SessionKeyLocation, form["Location"].ToString() ?? string.Empty);

            return RedirectToAction(nameof(Index));
        }

        private UserMatchmakingFilters LoadFiltersFromSession()
        {
            var filters = UserMatchmakingFilters.Empty();

            var empTypes = HttpContext.Session.GetString(SessionKeyEmploymentTypes);
            if (!string.IsNullOrEmpty(empTypes))
                foreach (var t in empTypes.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    filters.EmploymentTypes.Add(t);

            var expLevels = HttpContext.Session.GetString(SessionKeyExperienceLevels);
            if (!string.IsNullOrEmpty(expLevels))
                foreach (var l in expLevels.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    filters.ExperienceLevels.Add(l);

            filters.LocationSubstring = HttpContext.Session.GetString(SessionKeyLocation) ?? string.Empty;

            return filters;
        }
    }
}
