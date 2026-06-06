using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.JobSkills;
using UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;
using UBB_SE_2026_Jobs.Library.Services.UserProfileService;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize(Roles = "Candidate")]
public class UserStatusController : Controller
{
    private readonly IUserStatusService userStatus;
    private readonly IUserProfileService userProfileService;
    private readonly IUserSkillService userSkillService;
    private readonly IJobSkillService jobSkillService;
    private readonly IJobService jobService;
    private readonly RecommendationAlgorithm algorithm = new();

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public UserStatusController(
        IUserStatusService userStatus,
        IUserProfileService userProfileService,
        IUserSkillService userSkillService,
        IJobSkillService jobSkillService,
        IJobService jobService)
    {
        this.userStatus = userStatus;
        this.userProfileService = userProfileService;
        this.userSkillService = userSkillService;
        this.jobSkillService = jobSkillService;
        this.jobService = jobService;
    }

    public async Task<IActionResult> Index(string? filter, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;

        var applicationsTask = userStatus.GetApplicationsForUserAsync(userId, cancellationToken);
        var userTask = userProfileService.GetProfileAsync(userId, cancellationToken);
        var userSkillsTask = userSkillService.GetByUserIdAsync(userId, cancellationToken);

        await Task.WhenAll(applicationsTask, userTask, userSkillsTask);

        var applications = applicationsTask.Result;
        var user = userTask.Result;
        var userSkills = userSkillsTask.Result;

        if (user is not null)
            await RecalculateScoresAsync(applications, user, userSkills, cancellationToken);

        var filtered = filter switch
        {
            "Applied"  => applications.Where(a => a.Status == MatchStatus.Applied).ToList(),
            "Accepted" => applications.Where(a => a.Status == MatchStatus.Accepted).ToList(),
            "Rejected" => applications.Where(a => a.Status == MatchStatus.Rejected).ToList(),
            _          => applications.ToList(),
        };

        ViewBag.CurrentFilter = filter ?? "All";
        ViewBag.TotalCount = applications.Count;
        return View(filtered);
    }

    private async Task RecalculateScoresAsync(
        IReadOnlyList<ApplicationCardModel> applications,
        User user,
        IReadOnlyList<UserSkill> userSkills,
        CancellationToken cancellationToken)
    {
        var jobSkillTasks = applications
            .Select(app => jobSkillService.GetByJobIdAsync(app.JobId, cancellationToken))
            .ToList();

        var jobTasks = applications
            .Select(app => jobService.GetByIdAsync(app.JobId, cancellationToken))
            .ToList();

        await Task.WhenAll(jobSkillTasks.Cast<Task>().Concat(jobTasks.Cast<Task>()));

        for (var i = 0; i < applications.Count; i++)
        {
            var job = jobTasks[i].Result;
            if (job is null)
                continue;

            var jobSkills = jobSkillTasks[i].Result;
            var score = algorithm.CalculateCompatibilityScore(user, job, userSkills, jobSkills);
            applications[i].CompatibilityScore = (int)Math.Round(score);
        }
    }
}
