using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Repositories.Skills;
using UBB_SE_2026_Jobs.Library.Repositories.Users;

namespace UBB_SE_2026_Jobs.Library.Services.CompatibilityService;

public class CompatibilityService : ICompatibilityService
{
    private const int SkillsLineIndex = 2;
    private const char SkillDelimiter = ',';
    private const double ProfileSkillDefaultScore = 10.0;
    private const double ScoreNormalizationFactor = 100.0;
    private const double HighSkillCoverageThreshold = 0.5;
    private const double TargetGroupScore = 0.8;
    private const int MaxSuggestions = 3;
    private const int InvalidScore = -1;
    private const double TestScoreThreshold = 10.0;

    private readonly IUserSkillRepository userSkillRepository;
    private readonly ISkillGroupRepository skillGroupRepository;
    private readonly IUserRepository userRepository;
    private readonly ITestAttemptRepository testAttemptRepository;

    public CompatibilityService(
        IUserSkillRepository userSkillRepository,
        ISkillGroupRepository skillGroupRepository,
        IUserRepository userRepository,
        ITestAttemptRepository testAttemptRepository)
    {
        this.userSkillRepository = userSkillRepository;
        this.skillGroupRepository = skillGroupRepository;
        this.userRepository = userRepository;
        this.testAttemptRepository = testAttemptRepository;
    }

    public async Task<RoleResult> CalculateForRoleAsync(int userId, JobRole role, CancellationToken cancellationToken = default)
    {
        var allGroups = await skillGroupRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var userSkills = await GetUserSkillsAsync(userId, allGroups, cancellationToken).ConfigureAwait(false);
        var roleGroups = allGroups.Where(g => g.JobRole == role).ToList();
        return ComputeRole(role, userSkills, roleGroups);
    }

    public async Task<IReadOnlyList<RoleResult>> CalculateAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        // Load groups and user skills once, then compute each role in memory.
        var allGroups = await skillGroupRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var userSkills = await GetUserSkillsAsync(userId, allGroups, cancellationToken).ConfigureAwait(false);

        return Enum.GetValues<JobRole>()
            .Select(role => ComputeRole(role, userSkills, allGroups.Where(g => g.JobRole == role).ToList()))
            .ToList();
    }

    public IReadOnlyList<Suggestion> GetSuggestions(RoleResult result) => result.Suggestions;

    // ── Core role computation ────────────────────────────────────────────────

    private static RoleResult ComputeRole(JobRole role, List<CompatibilitySkill> userSkills, IReadOnlyList<SkillGroup> groups)
    {
        var groupScores = groups.Select(g => ComputeGroupScore(g, userSkills)).ToList();
        double matchScore = ComputeMatchScore(groups, groupScores);
        int totalWeight = groups.Sum(g => g.Weight);

        var result = new RoleResult { JobRole = role };

        if (matchScore == InvalidScore)
        {
            result.MatchScore = InvalidScore;
            result.Suggestions = new List<Suggestion>();
            result.ErrorMessage = "Compatibility calculation could not be completed. Please check your profile and skills.";
            return result;
        }

        result.MatchScore = matchScore;
        result.Suggestions = IdentifyGaps(groups, userSkills, totalWeight);
        result.SkillScores = BuildSkillScores(groups, userSkills);
        return result;
    }

    // ── User skill aggregation ───────────────────────────────────────────────

    private async Task<List<CompatibilitySkill>> GetUserSkillsAsync(
        int userId,
        IReadOnlyList<SkillGroup> allGroups,
        CancellationToken cancellationToken)
    {
        var profileSkills = await userSkillRepository.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        var user = await userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        var cvSkills = ExtractSkillsFromParsedCv(user?.ParsedCv ?? string.Empty);
        var skills = MergeProfileAndCvSkills(profileSkills, cvSkills);
        await ApplyBestTestAttemptScoresAsync(userId, skills, allGroups, cancellationToken).ConfigureAwait(false);
        return skills;
    }

    private static List<string> ExtractSkillsFromParsedCv(string parsedCv)
    {
        if (string.IsNullOrWhiteSpace(parsedCv))
            return new List<string>();

        string[] lines = parsedCv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        if (lines.Length <= SkillsLineIndex)
            return new List<string>();

        string skillsLine = lines[SkillsLineIndex].Trim();
        if (string.IsNullOrWhiteSpace(skillsLine))
            return new List<string>();

        return skillsLine.Split(SkillDelimiter)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }

    private static List<CompatibilitySkill> MergeProfileAndCvSkills(
        IReadOnlyList<UserSkill> profileSkills,
        List<string> cvSkills)
    {
        var allSkills = new List<CompatibilitySkill>();

        foreach (var profileSkill in profileSkills)
        {
            var skill = profileSkill.Skill;
            if (skill is null || string.IsNullOrWhiteSpace(skill.Name)) continue;

            AddOrImproveSkill(allSkills, new CompatibilitySkill(
                skill.SkillId,
                skill.Name,
                NormalizeScore(profileSkill.Score > 0 ? profileSkill.Score : ProfileSkillDefaultScore),
                profileSkill.IsVerified ? "Stored verified profile score" : "Profile skill baseline"));
        }

        foreach (string cvSkill in cvSkills)
        {
            AddOrImproveSkill(allSkills, new CompatibilitySkill(
                null,
                cvSkill,
                ProfileSkillDefaultScore,
                "CV skill baseline"));
        }

        return allSkills;
    }

    // Each seeded test maps 1-to-1 to a skill group (Test.Id == SkillGroup.SkillGroupId).
    // A test therefore represents competency across the *entire* group, not just the primary
    // skill stored in Test.SkillId. Apply the best attempt score to every skill in the group
    // so the group score reflects the user's actual test performance correctly.
    private async Task ApplyBestTestAttemptScoresAsync(
        int userId,
        List<CompatibilitySkill> skills,
        IReadOnlyList<SkillGroup> allGroups,
        CancellationToken cancellationToken)
    {
        var completedAttempts = await testAttemptRepository
            .FindCompletedByUserIdAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        // Best percentage score per test
        var bestPerTest = completedAttempts
            .Select(a => new { Attempt = a, Score = GetAttemptPercentageScore(a) })
            .Where(x => x.Score.HasValue && x.Score.Value > TestScoreThreshold)
            .GroupBy(x => x.Attempt.TestId)
            .Select(g => g.OrderByDescending(x => x.Score).First())
            .ToList();

        // TestId == SkillGroupId by seeding convention
        var groupByTestId = allGroups.ToDictionary(g => g.SkillGroupId);

        foreach (var best in bestPerTest)
        {
            if (!groupByTestId.TryGetValue(best.Attempt.TestId, out var group)) continue;

            var score = NormalizeScore(best.Score!.Value);
            var source = $"Best test attempt: {best.Attempt.Test?.Title ?? group.GroupName}";

            foreach (var skill in group.Skills)
            {
                AddOrImproveSkill(skills, new CompatibilitySkill(skill.SkillId, skill.Name, score, source));
            }
        }
    }

    private static double? GetAttemptPercentageScore(TestAttempt attempt)
    {
        if (attempt.PercentageScore.HasValue)
            return (double)attempt.PercentageScore.Value;

        var maximumPossibleScore = attempt.Test?.Questions.Sum(q => q.QuestionScore) ?? 0;
        if (!attempt.Score.HasValue || maximumPossibleScore <= 0)
            return null;

        return (double)attempt.Score.Value / maximumPossibleScore * ScoreNormalizationFactor;
    }

    // ── Scoring algorithms ───────────────────────────────────────────────────

    private static double ComputeGroupScore(SkillGroup group, List<CompatibilitySkill> userSkills)
    {
        if (group.Skills.Count == 0)
            return 0;

        double totalContribution = 0;
        int totalSkillsInGroup = group.Skills.Count;

        foreach (var skill in group.Skills)
        {
            var match = userSkills.FirstOrDefault(u => SkillMatches(u, skill));
            if (match is null) continue;
            totalContribution += (match.Score / ScoreNormalizationFactor) / totalSkillsInGroup;
        }

        return totalContribution;
    }

    private static double ComputeMatchScore(IReadOnlyList<SkillGroup> groups, List<double> groupScores)
    {
        int totalWeight = groups.Sum(g => g.Weight);
        if (totalWeight == 0) return InvalidScore;
        double weighted = groups.Select((g, i) => g.Weight * groupScores[i]).Sum();
        return weighted * ScoreNormalizationFactor / totalWeight;
    }

    private static List<Suggestion> IdentifyGaps(
        IReadOnlyList<SkillGroup> skillGroups,
        List<CompatibilitySkill> userSkills,
        int totalWeight)
    {
        var suggestions = new List<Suggestion>();

        foreach (var group in skillGroups)
        {
            double groupScore = ComputeGroupScore(group, userSkills);
            if (groupScore > HighSkillCoverageThreshold) continue;

            var missingSkill = group.Skills.FirstOrDefault(
                s => !userSkills.Any(u => SkillMatches(u, s)));
            if (missingSkill is null) continue;

            suggestions.Add(new Suggestion
            {
                SkillName = missingSkill.Name,
                GroupName = group.GroupName,
                GainScore = ScoreNormalizationFactor * group.Weight * (TargetGroupScore - groupScore) / totalWeight,
            });
        }

        return suggestions.OrderByDescending(s => s.GainScore).Take(MaxSuggestions).ToList();
    }

    private static List<CompatibilitySkillScore> BuildSkillScores(
        IReadOnlyList<SkillGroup> skillGroups,
        List<CompatibilitySkill> userSkills)
    {
        var roleSkillIds = skillGroups
            .SelectMany(g => g.Skills)
            .Select(s => s.SkillId)
            .ToHashSet();

        return userSkills
            .Where(u => skillGroups.Any(g => g.Skills.Any(s => SkillMatches(u, s))))
            .OrderByDescending(u => u.Score)
            .ThenBy(u => u.Name)
            .Select(u => new CompatibilitySkillScore
            {
                SkillName = u.Name,
                Score = u.Score,
                Source = u.Source,
            })
            .ToList();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static bool SkillMatches(CompatibilitySkill userSkill, Skill skill) =>
        userSkill.SkillId == skill.SkillId ||
        string.Equals(userSkill.Name, skill.Name, StringComparison.OrdinalIgnoreCase);

    private static void AddOrImproveSkill(List<CompatibilitySkill> skills, CompatibilitySkill candidate)
    {
        var existing = skills.FirstOrDefault(s =>
            string.Equals(s.Name, candidate.Name, StringComparison.OrdinalIgnoreCase) ||
            (s.SkillId is not null && s.SkillId == candidate.SkillId));

        if (existing is null)
        {
            skills.Add(candidate);
            return;
        }

        if (candidate.Score > existing.Score)
        {
            existing.Score = candidate.Score;
            existing.Source = candidate.Source;
        }

        existing.SkillId ??= candidate.SkillId;
    }

    private static double NormalizeScore(double score) => Math.Clamp(score, 0, ScoreNormalizationFactor);

    private sealed class CompatibilitySkill
    {
        public CompatibilitySkill(int? skillId, string name, double score, string source)
        {
            SkillId = skillId;
            Name = name;
            Score = score;
            Source = source;
        }

        public int? SkillId { get; set; }
        public string Name { get; }
        public double Score { get; set; }
        public string Source { get; set; }
    }
}
