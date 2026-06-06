using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Services.Matches;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize(Roles = "Candidate")]
public class MatchHistoryController : Controller
{
    private readonly IMatchService matchService;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public MatchHistoryController(IMatchService matchService)
    {
        this.matchService = matchService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var matches = await matchService.GetMatchesForUserAsync(CurrentUserId, cancellationToken);
        var statistics = await matchService.GetMatchStatisticsAsync(CurrentUserId, cancellationToken);

        ViewBag.Statistics = statistics;
        return View(matches);
    }
}