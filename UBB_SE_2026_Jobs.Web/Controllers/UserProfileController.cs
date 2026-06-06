using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.CompletenessService;
using UBB_SE_2026_Jobs.Library.Services.ImageStorage;
using UBB_SE_2026_Jobs.Library.Services.UserProfileService;
using UBB_SE_2026_Jobs.Library.Services.Users;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize(Roles = "Candidate")]
public class UserProfileController : Controller
{
    private readonly IUserProfileService userProfileService;
    private readonly ICompletenessService completenessService;
    private readonly IImageStorageService imageStorage;
    private readonly IUserService userService;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static int CalculateLevelProgress(int xp, int level) =>
         SimpleModelOperations.CalculateLevelProgress(xp, level);

    private static int CalculateXpToNextLevel(int xp, int level) =>
        SimpleModelOperations.CalculateXpToNextLevel(xp, level);

    public UserProfileController(
        IUserProfileService userProfileService,
        ICompletenessService completenessService,
        IImageStorageService imageStorage,
        IUserService userService)
    {
        this.userProfileService = userProfileService;
        this.completenessService = completenessService;
        this.imageStorage = imageStorage;
        this.userService = userService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await userProfileService.GetProfileAsync(CurrentUserId, cancellationToken);
        if (user is null)
            return NotFound();

        ViewBag.CompletenessPercentage = completenessService.CalculateCompleteness(user);
        ViewBag.NextFieldPrompt = completenessService.GetNextEmptyFieldPrompt(user);
        int totalXp = await userProfileService.RecalculateLevelAsync(user, cancellationToken);
        ViewBag.TotalXp = totalXp;
        ViewBag.LevelProgressPercent = CalculateLevelProgress(totalXp, user.CurrentLevel);
        ViewBag.XpToNextLevel = CalculateXpToNextLevel(totalXp, user.CurrentLevel);
        return View(user);
    }
   

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(CancellationToken cancellationToken)
    {
        var user = await userProfileService.GetProfileAsync(CurrentUserId, cancellationToken);
        if (user is null)
            return NotFound();

        await userProfileService.UpdateAccountStatusAsync(CurrentUserId, !user.ActiveAccount, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar, CancellationToken cancellationToken)
    {
        if (avatar is null || avatar.Length == 0)
        {
            TempData["Error"] = "Please select an image file.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await using var stream = avatar.OpenReadStream();
            imageStorage.CheckFileSize(stream);
            stream.Position = 0;
            var path = await imageStorage.SaveImageAsync(stream, avatar.FileName, cancellationToken);
            await userService.SetProfilePicturePathAsync(CurrentUserId, path, cancellationToken);
        }
        catch (Exception exception)
        {
            TempData["Error"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAvatar(CancellationToken cancellationToken)
    {
        var user = await userProfileService.GetProfileAsync(CurrentUserId, cancellationToken);
        if (user is not null && !string.IsNullOrWhiteSpace(user.ProfilePicturePath))
            await imageStorage.DeleteImageAsync(user.ProfilePicturePath, cancellationToken);

        await userService.SetProfilePicturePathAsync(CurrentUserId, string.Empty, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await userProfileService.GetProfileAsync(CurrentUserId, CancellationToken.None);

        if (user == null)
        {
            user = new User();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(User model)
    {
        if (model.WorkExperiences != null)
        {
            for (int workExperienceIndex = 0; workExperienceIndex < model.WorkExperiences.Count; workExperienceIndex++)
            {
                var workExperience = model.WorkExperiences[workExperienceIndex];
                if (string.IsNullOrWhiteSpace(workExperience.Company))
                    ModelState.AddModelError($"WorkExperiences[{workExperienceIndex}].Company", "Company is required.");
                if (string.IsNullOrWhiteSpace(workExperience.JobTitle))
                    ModelState.AddModelError($"WorkExperiences[{workExperienceIndex}].JobTitle", "Job title is required.");
                if (workExperience.StartDate == default)
                    ModelState.AddModelError($"WorkExperiences[{workExperienceIndex}].StartDate", "Start date is required.");
            }
        }

        if (model.Projects != null)
        {
            for (int projectIndex = 0; projectIndex < model.Projects.Count; projectIndex++)
            {
                var project = model.Projects[projectIndex];
                if (string.IsNullOrWhiteSpace(project.Name))
                    ModelState.AddModelError($"Projects[{projectIndex}].Name", "Project name is required.");
                if (string.IsNullOrWhiteSpace(project.Description))
                    ModelState.AddModelError($"Projects[{projectIndex}].Description", "Description is required.");
            }
        }

        if (model.ExtraCurricularActivities != null)
        {
            for (int activityIndex = 0; activityIndex < model.ExtraCurricularActivities.Count; activityIndex++)
            {
                var activity = model.ExtraCurricularActivities[activityIndex];
                if (string.IsNullOrWhiteSpace(activity.ActivityName))
                    ModelState.AddModelError($"ExtraCurricularActivities[{activityIndex}].ActivityName", "Activity name is required.");
                if (string.IsNullOrWhiteSpace(activity.Organization))
                    ModelState.AddModelError($"ExtraCurricularActivities[{activityIndex}].Organization", "Organization is required.");
                if (string.IsNullOrWhiteSpace(activity.Role))
                    ModelState.AddModelError($"ExtraCurricularActivities[{activityIndex}].Role", "Role is required.");
                if (string.IsNullOrWhiteSpace(activity.Period))
                    ModelState.AddModelError($"ExtraCurricularActivities[{activityIndex}].Period", "Period is required.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await userProfileService.SaveAsync(CurrentUserId, model, CancellationToken.None);

            TempData["Success"] = "Profile saved successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            return View(model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadCv(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { detail = "No file uploaded." });

        try
        {
            await using var stream = file.OpenReadStream();
            var user = await userProfileService.UploadCvAsync(CurrentUserId, stream, file.FileName, cancellationToken);
            return Json(user);
        }
        catch (Exception exception)
        {
            return BadRequest(new { detail = exception.Message });
        }
    }
}
