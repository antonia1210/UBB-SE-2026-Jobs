using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;

/// <summary>
/// Computes job-applicant compatibility for the recommendation services.
/// </summary>
public interface IRecommendationAlgorithm
{
    /// <summary>
    /// Calculates the aggregate compatibility score for a candidate and a job.
    /// </summary>
    double CalculateCompatibilityScore(
        User user,
        Job job,
        IReadOnlyList<UserSkill> userSkills,
        IReadOnlyList<JobSkill> jobSkills);

    /// <summary>
    /// Calculates the score plus its individual component breakdown.
    /// </summary>
    CompatibilityBreakdown CalculateScoreBreakdown(
        User user,
        Job job,
        IReadOnlyList<UserSkill> userSkills,
        IReadOnlyList<JobSkill> jobSkills);
}
