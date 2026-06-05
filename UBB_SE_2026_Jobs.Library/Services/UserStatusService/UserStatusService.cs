using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Repositories.Matches;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.JobSkills;
using UBB_SE_2026_Jobs.Library.Services.PussyCatsCompanyService;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;

namespace UBB_SE_2026_Jobs.Library.Services.UserStatusService;

public class UserStatusService : IUserStatusService
{
    private const int FullCompatibilityScore = 100;
    private const int PercentageMultiplier = 100;

    private readonly IMatchRepository matchRepository;
    private readonly IPussyCatsJobService jobService;
    private readonly IPussyCatsCompanyService companyService;
    private readonly IUserSkillService userSkillService;
    private readonly IJobSkillService jobSkillService;

    public UserStatusService(
        IMatchRepository matchRepository,
        IPussyCatsJobService jobService,
        IPussyCatsCompanyService companyService,
        IUserSkillService userSkillService,
        IJobSkillService jobSkillService)
    {
        this.matchRepository = matchRepository;
        this.jobService = jobService;
        this.companyService = companyService;
        this.userSkillService = userSkillService;
        this.jobSkillService = jobSkillService;
    }

    public async Task<IReadOnlyList<ApplicationCardModel>> GetApplicationsForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var matches = await matchRepository.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        var userSkills = await userSkillService.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

        var jobsById = (await jobService.GetAllAsync(cancellationToken).ConfigureAwait(false))
            .ToDictionary(job => job.JobId);
        var companiesById = (await companyService.GetAllAsync(cancellationToken).ConfigureAwait(false))
            .ToDictionary(company => company.CompanyId);
        var jobSkillsByJobId = (await jobSkillService.GetAllAsync(cancellationToken).ConfigureAwait(false))
            .GroupBy(jobSkill => jobSkill.Job.JobId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<JobSkill>)group.ToList());

        var result = new List<ApplicationCardModel>();
        foreach (var match in matches)
        {
            if (!jobsById.TryGetValue(match.Job.JobId, out var job))
                continue;

            companiesById.TryGetValue(job.Company.CompanyId, out var company);
            var jobSkills = jobSkillsByJobId.GetValueOrDefault(match.Job.JobId) ?? [];
            var compatibilityScore = CalculateCompatibilityScore(userSkills, jobSkills);

            result.Add(new ApplicationCardModel
            {
                MatchId = match.MatchId,
                JobId = match.Job.JobId,
                CompanyName = company?.Name ?? "Unknown Company",
                JobDescription = job.JobDescription,
                AppliedDate = match.Timestamp,
                Status = match.Status,
                CompatibilityScore = compatibilityScore,
                FeedbackMessage = match.FeedbackMessage,
            });
        }

        return result;
    }

    private static int CalculateCompatibilityScore(IReadOnlyList<UserSkill> userSkills, IReadOnlyList<JobSkill> jobSkills)
    {
        if (jobSkills.Count == 0)
            return FullCompatibilityScore;

        var userSkillScoreBySkillId = userSkills.ToDictionary(userSkill => userSkill.Skill.SkillId, userSkill => userSkill.Score);

        double totalScore = 0;
        foreach (var requiredJobSkill in jobSkills)
        {
            if (userSkillScoreBySkillId.TryGetValue(requiredJobSkill.Skill.SkillId, out var userScore))
                totalScore += Math.Min(userScore, requiredJobSkill.RequiredLevel) / (double)requiredJobSkill.RequiredLevel;
        }

        return (int)(totalScore / jobSkills.Count * PercentageMultiplier);
    }
}