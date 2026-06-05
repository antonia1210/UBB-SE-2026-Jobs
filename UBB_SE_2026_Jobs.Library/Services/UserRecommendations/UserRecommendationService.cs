using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Repositories.Companies;
using UBB_SE_2026_Jobs.Library.Repositories.Jobs;
using UBB_SE_2026_Jobs.Library.Repositories.Recommendations;
using UBB_SE_2026_Jobs.Library.Repositories.Skills;
using UBB_SE_2026_Jobs.Library.Repositories.Users;
using UBB_SE_2026_Jobs.Library.Services.CooldownService;
using UBB_SE_2026_Jobs.Library.Services.Matches;
using UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;
using UBB_SE_2026_Jobs.Library.Services.UserRecommendationService;

namespace UBB_SE_2026_Jobs.Library.Services.UserRecommendations;

public sealed class UserRecommendationService : IUserRecommendationService
{
    private const int InternshipYearsThreshold = 2;
    private const int EntryYearsThreshold = 4;
    private const int MidSeniorYearsThreshold = 7;
    private const int DirectorYearsThreshold = 10;

    private readonly IUserRepository userRepository;
    private readonly IPussyCatsJobRepository jobRepository;
    private readonly IUserSkillRepository userSkillRepository;
    private readonly IJobSkillRepository jobSkillRepository;
    private readonly IPussyCatsCompanyRepository companyRepository;
    private readonly IMatchService matchService;
    private readonly IRecommendationRepository recommendationRepository;
    private readonly ICooldownService cooldownService;
    private readonly IRecommendationAlgorithm algorithm;

    public UserRecommendationService(
        IUserRepository userRepository,
        IPussyCatsJobRepository jobRepository,
        IUserSkillRepository userSkillRepository,
        IJobSkillRepository jobSkillRepository,
        IPussyCatsCompanyRepository companyRepository,
        IMatchService matchService,
        IRecommendationRepository recommendationRepository,
        ICooldownService cooldownService,
        IRecommendationAlgorithm algorithm)
    {
        this.userRepository = userRepository;
        this.jobRepository = jobRepository;
        this.userSkillRepository = userSkillRepository;
        this.jobSkillRepository = jobSkillRepository;
        this.companyRepository = companyRepository;
        this.matchService = matchService;
        this.recommendationRepository = recommendationRepository;
        this.cooldownService = cooldownService;
        this.algorithm = algorithm;
    }

    public async Task<JobRecommendationResult?> GetNextCardAsync(int userId, UserMatchmakingFilters filters, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");

        var rankedJobs = await BuildRankedListAsync(user, filters, cancellationToken).ConfigureAwait(false);
        if (rankedJobs.Count == 0)
        {
            return null;
        }

        var (topRankedJob, topScore) = rankedJobs[0];
        return await CreateCardAsync(topRankedJob, topScore, null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<JobRecommendationResult?> RecalculateTopCardIgnoringCooldownAsync(int userId, UserMatchmakingFilters filters, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");

        var rankedJobs = await BuildRankedListIgnoringCooldownAsync(user, filters, cancellationToken).ConfigureAwait(false);
        if (rankedJobs.Count == 0)
        {
            return null;
        }

        var bestRankedJob = rankedJobs[0];
        return await CreateCardAsync(bestRankedJob.Job, bestRankedJob.Score, null, cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<(Job Job, double Score)>> BuildRankedListIgnoringCooldownAsync(User user, UserMatchmakingFilters filters, CancellationToken cancellationToken)
    {
        var filteredJobs = await GetFilteredJobsAsync(filters, user, cancellationToken).ConfigureAwait(false);
        var userSkills = await userSkillRepository.GetByUserIdAsync(user.UserId, cancellationToken).ConfigureAwait(false);

        var rankedJobs = new List<(Job Job, double Score)>();
        foreach (var currentJob in filteredJobs)
        {
            if (await matchService.GetByUserIdAndJobIdAsync(user.UserId, currentJob.JobId, cancellationToken).ConfigureAwait(false) is not null)
            {
                continue;
            }

            var compatibilityScore = await ComputeCompatibilityScoreAsync(user, currentJob, userSkills, cancellationToken).ConfigureAwait(false);
            rankedJobs.Add((currentJob, compatibilityScore));
        }

        rankedJobs.Sort(CompareRankedJobsByScoreDescending);
        return rankedJobs;
    }

    private async Task<double> ComputeCompatibilityScoreAsync(User user, Job job, IReadOnlyList<UserSkill> userSkills, CancellationToken cancellationToken)
    {
        var jobSkills = await jobSkillRepository.GetByJobIdAsync(job.JobId, cancellationToken).ConfigureAwait(false);
        return algorithm.CalculateCompatibilityScore(user, job, userSkills, jobSkills);
    }

    private async Task<List<(Job Job, double Score)>> BuildRankedListAsync(User user, UserMatchmakingFilters filters, CancellationToken cancellationToken)
    {
        var filteredJobs = await GetFilteredJobsAsync(filters, user, cancellationToken).ConfigureAwait(false);
        var userSkills = await userSkillRepository.GetByUserIdAsync(user.UserId, cancellationToken).ConfigureAwait(false);

        var rankedJobs = new List<(Job Job, double Score)>();
        foreach (var currentJob in filteredJobs)
        {
            if (await matchService.GetByUserIdAndJobIdAsync(user.UserId, currentJob.JobId, cancellationToken).ConfigureAwait(false) is not null)
            {
                continue;
            }

            if (await cooldownService.IsOnCooldownAsync(user.UserId, currentJob.JobId, DateTime.UtcNow, cancellationToken).ConfigureAwait(false))
            {
                continue;
            }

            var compatibilityScore = await ComputeCompatibilityScoreAsync(user, currentJob, userSkills, cancellationToken).ConfigureAwait(false);
            rankedJobs.Add((currentJob, compatibilityScore));
        }

        rankedJobs.Sort(CompareRankedJobsByScoreDescending);
        return rankedJobs;
    }

    private async Task<JobRecommendationResult> BuildCardWithShownRecordAsync(User user, Job job, double score, CancellationToken cancellationToken)
    {
        var displayRecommendation = new Recommendation
        {
            User = user,
            Job = job,
            Timestamp = DateTime.UtcNow,
        };

        var savedRecommendation = await recommendationRepository.AddAsync(displayRecommendation, cancellationToken).ConfigureAwait(false);
        return await CreateCardAsync(job, score, savedRecommendation.RecommendationId, cancellationToken).ConfigureAwait(false);
    }

    private async Task<JobRecommendationResult> CreateCardAsync(Job job, double score, int? displayRecommendationId, CancellationToken cancellationToken)
    {
        var company = await companyRepository.GetByIdAsync(job.Company.CompanyId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Company {job.Company.CompanyId} not found.");

        var jobSkillRows = await jobSkillRepository.GetByJobIdAsync(job.JobId, cancellationToken).ConfigureAwait(false);
        var topSkills = JobRecommendationResult.TakeTopSkills(jobSkillRows);
        var allSkillLabels = new List<string>();
        foreach (var jobSkill in jobSkillRows)
        {
            var skillName = jobSkill.Skill?.Name ?? $"Skill #{jobSkill.Skill?.SkillId ?? 0}";
            allSkillLabels.Add($"{skillName} (min {jobSkill.RequiredLevel})");
        }

        return new JobRecommendationResult
        {
            Job = job,
            Company = company,
            CompatibilityScore = score,
            TopSkillLabels = topSkills,
            AllSkillLabels = allSkillLabels,
            DisplayRecommendationId = displayRecommendationId,
        };
    }

    public async Task<int> ApplyLikeAsync(int userId, JobRecommendationResult card, CancellationToken cancellationToken = default)
    {
        var targetJob = card.Job;
        if (await matchService.GetByUserIdAndJobIdAsync(userId, targetJob.JobId, cancellationToken).ConfigureAwait(false) is not null)
        {
            throw new InvalidOperationException("Already applied to this job.");
        }

        return await matchService.CreatePendingApplicationAsync(userId, targetJob.JobId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> ApplyDismissAsync(int userId, JobRecommendationResult card, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");

        var dismissedRecommendation = new Recommendation
        {
            User = user,
            Job = card.Job,
            Timestamp = DateTime.UtcNow,
        };

        var savedRecommendation = await recommendationRepository.AddAsync(dismissedRecommendation, cancellationToken).ConfigureAwait(false);
        return savedRecommendation.RecommendationId;
    }

    public async Task UndoLikeAsync(int matchId, int? displayRecommendationId, CancellationToken cancellationToken = default)
    {
        await matchService.RemoveApplicationAsync(matchId, cancellationToken).ConfigureAwait(false);
        if (displayRecommendationId is { } resolvedDisplayRecommendationId)
        {
            await recommendationRepository.RemoveAsync(resolvedDisplayRecommendationId, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task UndoDismissAsync(int dismissRecommendationId, int? displayRecommendationId, CancellationToken cancellationToken = default)
    {
        await recommendationRepository.RemoveAsync(dismissRecommendationId, cancellationToken).ConfigureAwait(false);
        if (displayRecommendationId is { } resolvedDisplayRecommendationId && resolvedDisplayRecommendationId != dismissRecommendationId)
        {
            await recommendationRepository.RemoveAsync(resolvedDisplayRecommendationId, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<bool> PassesFiltersAsync(Job job, UserMatchmakingFilters filters, User user, CancellationToken cancellationToken)
    {
        if (filters.EmploymentTypes.Count > 0)
        {
            if (job.JobType is null || !filters.EmploymentTypes.Contains(job.JobType))
            {
                return false;
            }
        }

        if (filters.ExperienceLevels.Count > 0)
        {
            if (job.ExperienceLevel is null || !filters.ExperienceLevels.Contains(job.ExperienceLevel))
            {
                return false;
            }
        }

        if (filters.WorkModes.Count > 0)
        {
            if (job.JobLocation is null || !filters.WorkModes.Contains(job.JobLocation))
            {
                return false;
            }
        }

        if (!string.IsNullOrWhiteSpace(filters.LocationSubstring))
        {
            if ((job.JobLocation ?? string.Empty).IndexOf(filters.LocationSubstring.Trim(), StringComparison.OrdinalIgnoreCase) < 0)
            {
                return false;
            }
        }

        if (filters.SkillIds.Count > 0)
        {
            var jobSkillIds = await GetJobSkillIdSetAsync(job.JobId, cancellationToken).ConfigureAwait(false);
            if (!HasAnySkillIntersection(filters.SkillIds, jobSkillIds))
            {
                return false;
            }
        }

        return true;
    }

    public static string MapUserYearsToExperienceBucket(int yearsOfExperience)
    {
        return yearsOfExperience switch
        {
            < InternshipYearsThreshold => "Internship",
            < EntryYearsThreshold => "Entry",
            < MidSeniorYearsThreshold => "MidSenior",
            < DirectorYearsThreshold => "Director",
            _ => "Executive",
        };
    }

    private async Task<List<Job>> GetFilteredJobsAsync(UserMatchmakingFilters filters, User user, CancellationToken cancellationToken)
    {
        var filteredJobs = new List<Job>();
        foreach (var job in await jobRepository.GetAllAsync(cancellationToken).ConfigureAwait(false))
        {
            if (await PassesFiltersAsync(job, filters, user, cancellationToken).ConfigureAwait(false))
            {
                filteredJobs.Add(job);
            }
        }

        return filteredJobs;
    }

    private async Task<HashSet<int>> GetJobSkillIdSetAsync(int jobId, CancellationToken cancellationToken)
    {
        var skillIds = new HashSet<int>();
        foreach (var jobSkill in await jobSkillRepository.GetByJobIdAsync(jobId, cancellationToken).ConfigureAwait(false))
        {
            skillIds.Add(jobSkill.Skill.SkillId);
        }

        return skillIds;
    }

    private static bool HasAnySkillIntersection(IReadOnlyCollection<int> filterSkillIds, HashSet<int> jobSkillIds)
    {
        foreach (var filterSkillId in filterSkillIds)
        {
            if (jobSkillIds.Contains(filterSkillId))
            {
                return true;
            }
        }

        return false;
    }

    private static int CompareRankedJobsByScoreDescending((Job Job, double Score) left, (Job Job, double Score) right)
    {
        return right.Score.CompareTo(left.Score);
    }
}