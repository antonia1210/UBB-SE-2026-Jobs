using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Services.CompanyStatusService;
using UBB_SE_2026_Jobs.Library.Services.Matches;
using UBB_SE_2026_Jobs.Web.Models;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize(Roles = "Recruiter")]
public class CompanyStatusController : Controller
{
    private readonly ICompanyStatusService companyStatusService;
    private readonly IMatchService matchService;

    public CompanyStatusController(
        ICompanyStatusService companyStatusService,
        IMatchService matchService)
    {
        this.companyStatusService = companyStatusService;
        this.matchService = matchService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var applicants = await companyStatusService
            .GetApplicantsForCompanyAsync(GetCompanyId(), cancellationToken);
        return View(applicants);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var applicant = await companyStatusService
            .GetApplicantByMatchIdAsync(GetCompanyId(), id, cancellationToken);
        return applicant is null ? NotFound() : View(applicant);
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var applicant = await companyStatusService
            .GetApplicantByMatchIdAsync(GetCompanyId(), id, cancellationToken);
        if (applicant is null)
        {
            return NotFound();
        }

        var model = new MatchDecisionFormModel
        {
            MatchId = applicant.Match.MatchId,
            Decision = applicant.Match.Status,
            Feedback = applicant.Match.FeedbackMessage,
            ApplicantName = applicant.User.Name,
            JobTitle = applicant.Job.JobTitle,
            CompanyName = applicant.Job.Company.Name,
            CurrentStatus = applicant.Match.Status,
            Timestamp = applicant.Match.Timestamp,
        };

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MatchDecisionFormModel model, CancellationToken cancellationToken)
    {
        if (id != model.MatchId)
        {
            return BadRequest();
        }

        if (!TryValidateDecision(model))
        {
            return View(model);
        }

        await matchService.SubmitDecisionAsync(
            model.MatchId,
            model.Decision!.Value,
            model.Feedback.Trim(),
            cancellationToken);

        return RedirectToAction(nameof(Details), new { id = model.MatchId });
    }

    private int GetCompanyId()
    {
        var value = User.FindFirstValue("CompanyId");
        if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var companyId))
            throw new InvalidOperationException("Recruiter session is missing a company ID.");
        return companyId;
    }

    private bool TryValidateDecision(MatchDecisionFormModel model)
    {
        if (model.Decision is not (UBB_SE_2026_Jobs.Library.Domain.Enums.MatchStatus.Accepted
            or UBB_SE_2026_Jobs.Library.Domain.Enums.MatchStatus.Rejected))
        {
            ModelState.AddModelError(nameof(model.Decision), "Select either Accepted or Rejected.");
        }

        if (string.IsNullOrWhiteSpace(model.Feedback))
        {
            ModelState.AddModelError(nameof(model.Feedback), "Feedback is required.");
        }
        else if (model.Feedback.Trim().Length > MatchDecisionFormModel.MaximumFeedbackLength)
        {
            ModelState.AddModelError(nameof(model.Feedback), "Feedback must be 500 characters or fewer.");
        }

        return ModelState.IsValid;
    }
}
