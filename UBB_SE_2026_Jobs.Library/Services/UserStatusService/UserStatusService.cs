using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Repositories.Matches;
using UBB_SE_2026_Jobs.Library.Repositories.Users;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.JobSkills;
using UBB_SE_2026_Jobs.Library.Services.PussyCatsCompanyService;
using UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;

namespace UBB_SE_2026_Jobs.Library.Services.UserStatusService;

public class UserStatusService : IUserStatusService
{
    private readonly IMatchRepository matchRepository;
    private readonly IPussyCatsJobService jobService;
    private readonly IPussyCatsCompanyService companyService;
    private readonly IUserSkillService userSkillService;
    private readonly IJobSkillService jobSkillService;
    private readonly IUserRepository userRepository;
    private readonly IRecommendationAlgorithm algorithm;

    public UserStatusService(
        IMatchRepository matchRepository,
        IPussyCatsJobService jobService,
        IPussyCatsCompanyService companyService,
        IUserSkillService userSkillService,
        IJobSkillService jobSkillService,
        IUserRepository userRepository,
        IRecommendationAlgorithm algorithm)
    {
        this.matchRepository = matchRepository;
        this.jobService = jobService;
        this.companyService = companyService;
        this.userSkillService = userSkillService;
        this.jobSkillService = jobSkillService;
        this.userRepository = userRepository;
        this.algorithm = algorithm;
    }

    public async Task<IReadOnlyList<ApplicationCardModel>> GetApplicationsForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var matches = await matchRepository.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        var user = await userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
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

            var compatibilityScore = user is not null
                ? (int)algorithm.CalculateCompatibilityScore(user, job, userSkills, jobSkills)
                : 0;

            result.Add(new ApplicationCardModel
            {
                MatchId = match.MatchId,
                JobId = match.Job.JobId,
                CompanyName = company?.Name ?? "Unknown Company",
                JobDescription = job.JobDescription ?? string.Empty,
                AppliedDate = match.Timestamp,
                Status = match.Status,
                CompatibilityScore = compatibilityScore,
                FeedbackMessage = match.FeedbackMessage ?? string.Empty,
            });
        }

        return result;
    }
}
