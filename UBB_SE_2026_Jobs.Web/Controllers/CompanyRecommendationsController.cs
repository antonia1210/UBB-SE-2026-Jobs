using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Services.CompanyRecommendationService;
using UBB_SE_2026_Jobs.Library.Services.Matches;
using UBB_SE_2026_Jobs.Web.Models;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize(Roles = "Recruiter")]
public class CompanyRecommendationsController : Controller
{
    private readonly ICompanyRecommendationService companyRecommendations;
    private readonly IMatchService matches;

    public CompanyRecommendationsController(
        ICompanyRecommendationService companyRecommendations,
        IMatchService matches)
    {
        this.companyRecommendations = companyRecommendations;
        this.matches = matches;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var applicants = await companyRecommendations.GetRankedApplicantsAsync(
            GetCompanyId(),
            cancellationToken);
        return View(applicants);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var applicant = await companyRecommendations.GetApplicantByMatchIdAsync(
            GetCompanyId(),
            id,
            cancellationToken);

        if (applicant is null)
        {
            return NotFound();
        }

        var breakdown = await companyRecommendations.GetBreakdownAsync(applicant, cancellationToken);
        return View(new CompanyRecommendationDetailsModel
        {
            Applicant = applicant,
            Breakdown = breakdown,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Advance(int id, bool returnToDetails, CancellationToken cancellationToken)
    {
        return await ReviewApplicantAsync(
            id,
            returnToDetails,
            true,
            () => matches.AdvanceAsync(id, cancellationToken),
            cancellationToken);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Skip(int id, bool returnToDetails, CancellationToken cancellationToken)
    {
        return await ReviewApplicantAsync(
            id,
            returnToDetails,
            false,
            () => matches.RejectAsync(id, "Rejected on first pass", cancellationToken),
            cancellationToken);
    }

    private async Task<IActionResult> ReviewApplicantAsync(
        int matchId,
        bool returnToDetails,
        bool isAdvance,
        Func<Task> action,
        CancellationToken cancellationToken)
    {
        var applicant = await companyRecommendations.GetApplicantByMatchIdAsync(
            GetCompanyId(),
            matchId,
            cancellationToken);

        if (applicant is null)
        {
            return NotFound();
        }

        try
        {
            await action();
            TempData["SuccessMessage"] = isAdvance
                ? "Applicant advanced to the next stage."
                : "Applicant skipped.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) when (IsReviewActionException(exception))
        {
            TempData["ErrorMessage"] = exception.Message;
            return returnToDetails
                ? RedirectToAction(nameof(Details), new { id = matchId })
                : RedirectToAction(nameof(Index));
        }
    }

    private static bool IsReviewActionException(Exception exception)
    {
        return exception is InvalidOperationException or KeyNotFoundException
            || exception is HttpRequestException
            {
                StatusCode: HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.UnprocessableEntity,
            };
    }

    private int GetCompanyId()
    {
        var value = User.FindFirstValue("CompanyId");
        if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var companyId))
            throw new InvalidOperationException("Recruiter session is missing a company ID.");
        return companyId;
    }
}
