using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Services.CompanyRecommendationService;
using UBB_SE_2026_Jobs.Web.Models;

namespace UBB_SE_2026_Jobs.Web.Controllers;

[Authorize(Roles = "Recruiter")]
public class CompanyRecommendationsController : Controller
{
    private readonly ICompanyRecommendationService companyRecommendations;

    public CompanyRecommendationsController(ICompanyRecommendationService companyRecommendations)
    {
        this.companyRecommendations = companyRecommendations;
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

    private int GetCompanyId()
    {
        var value = User.FindFirstValue("CompanyId");
        if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var companyId))
            throw new InvalidOperationException("Recruiter session is missing a company ID.");
        return companyId;
    }
}
