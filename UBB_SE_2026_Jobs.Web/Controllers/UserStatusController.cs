using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize(Roles = "Candidate")]
public class UserStatusController : Controller
{
    private readonly IUserStatusService userStatus;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public UserStatusController(IUserStatusService userStatus)
    {
        this.userStatus = userStatus;
    }

    public async Task<IActionResult> Index(string? filter, CancellationToken cancellationToken)
    {
        var applications = await userStatus.GetApplicationsForUserAsync(CurrentUserId, cancellationToken);

        var filtered = filter switch
        {
            "Applied"  => applications.Where(applicationToCheckForStatus => applicationToCheckForStatus.Status == MatchStatus.Applied).ToList(),
            "Accepted" => applications.Where(applicationToCheckForStatus => applicationToCheckForStatus.Status == MatchStatus.Accepted).ToList(),
            "Rejected" => applications.Where(applicationToCheckForStatus => applicationToCheckForStatus.Status == MatchStatus.Rejected).ToList(),
            _          => applications.ToList(),
        };

        ViewBag.CurrentFilter = filter ?? "All";
        ViewBag.TotalCount = applications.Count;
        return View(filtered);
    }
}
