using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.Matches;
using UBB_SE_2026_Jobs.Library.Services.Users;
using UBB_SE_2026_Jobs.Library.Services.JobSkills;
using UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;

namespace UBB_SE_2026_Jobs.Library.Services.CompanyRecommendationService;

public class CompanyRecommendationService : ICompanyRecommendationService
{
    private readonly IMatchService matchService;
    private readonly IUserService userService;
    private readonly IPussyCatsJobService jobService;
    private readonly IUserSkillService userSkillService;
    private readonly IJobSkillService jobSkillService;
    private readonly IRecommendationAlgorithm algorithm;

    private List<UserApplicationResult> rankedApplicants = new List<UserApplicationResult>();
    private int currentApplicantIndex;

    public CompanyRecommendationService(
        IMatchService matchService,
        IUserService userService,
        IPussyCatsJobService jobService,
        IUserSkillService userSkillService,
        IJobSkillService jobSkillService,
        IRecommendationAlgorithm algorithm)
    {
        this.matchService = matchService;
        this.userService = userService;
        this.jobService = jobService;
        this.userSkillService = userSkillService;
        this.jobSkillService = jobSkillService;
        this.algorithm = algorithm;
    }

    public async Task<IReadOnlyList<UserApplicationResult>> GetRankedApplicantsAsync(int companyId, CancellationToken cancellationToken = default)
    {
        var companyJobs = await jobService.GetByCompanyIdAsync(companyId, cancellationToken).ConfigureAwait(false);
        var companyJobIds = GetJobIds(companyJobs);

        if (companyJobIds.Count == 0)
        {
            return new List<UserApplicationResult>();
        }

        var allMatches = await matchService.GetAllMatchesAsync(cancellationToken).ConfigureAwait(false);
        var appliedMatches = new List<Match>();
        foreach (var match in allMatches)
        {
            if (match.Status == MatchStatus.Applied && companyJobIds.Contains(match.Job.JobId))
            {
                appliedMatches.Add(match);
            }
        }

        var results = new List<UserApplicationResult>();
        foreach (var match in appliedMatches)
        {
            var user = match.User;
            var job = await jobService.GetByIdAsync(match.Job.JobId, cancellationToken).ConfigureAwait(false);
            if (job is null)
            {
                continue;
            }

            var userSkills = await userSkillService.GetByUserIdAsync(user.UserId, cancellationToken).ConfigureAwait(false);
            var jobSkills = await jobSkillService.GetByJobIdAsync(job.JobId, cancellationToken).ConfigureAwait(false);

            var score = algorithm.CalculateCompatibilityScore(user, job, userSkills, jobSkills);

            results.Add(new UserApplicationResult
            {
                User = user,
                Match = match,
                Job = job,
                CompatibilityScore = score,
                UserSkills = userSkills,
                Feedback = match.FeedbackMessage,
            });
        }

        results.Sort(CompareByCompatibilityScoreDescending);
        return results;
    }

    public async Task<UserApplicationResult?> GetApplicantByMatchIdAsync(int companyId, int matchId, CancellationToken cancellationToken = default)
    {
        var applicants = await GetRankedApplicantsAsync(companyId, cancellationToken).ConfigureAwait(false);
        foreach (var applicant in applicants)
        {
            if (applicant.Match.MatchId == matchId)
            {
                return applicant;
            }
        }

        return null;
    }

    public async Task LoadApplicantsAsync(int companyId, CancellationToken cancellationToken = default)
    {
        rankedApplicants = (await GetRankedApplicantsAsync(companyId, cancellationToken).ConfigureAwait(false)).ToList();
        currentApplicantIndex = 0;
    }

    public UserApplicationResult? GetNextApplicant()
    {
        if (currentApplicantIndex >= rankedApplicants.Count)
        {
            return null;
        }

        return rankedApplicants[currentApplicantIndex];
    }

    public void MoveToNext()
    {
        currentApplicantIndex++;
    }

    public void MoveToPrevious()
    {
        if (currentApplicantIndex > 0)
        {
            currentApplicantIndex--;
        }
    }

    public bool HasMore => currentApplicantIndex < rankedApplicants.Count;

    public async Task<CompatibilityBreakdown?> GetBreakdownAsync(UserApplicationResult applicant, CancellationToken cancellationToken = default)
    {
        var jobSkills = await jobSkillService.GetByJobIdAsync(applicant.Job.JobId, cancellationToken).ConfigureAwait(false);

        return algorithm.CalculateScoreBreakdown(
            applicant.User,
            applicant.Job,
            applicant.UserSkills,
            jobSkills);
    }

    private static HashSet<int> GetJobIds(IReadOnlyList<Job> companyJobs)
    {
        var jobIds = new HashSet<int>();
        foreach (var job in companyJobs)
        {
            jobIds.Add(job.JobId);
        }

        return jobIds;
    }

    private static int CompareByCompatibilityScoreDescending(UserApplicationResult firstApplicant, UserApplicationResult secondApplicant)
    {
        return secondApplicant.CompatibilityScore.CompareTo(firstApplicant.CompatibilityScore);
    }
}