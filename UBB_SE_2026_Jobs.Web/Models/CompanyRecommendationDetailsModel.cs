using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Web.Models;

public class CompanyRecommendationDetailsModel
{
    public required UserApplicationResult Applicant { get; set; }

    public CompatibilityBreakdown? Breakdown { get; set; }
}
