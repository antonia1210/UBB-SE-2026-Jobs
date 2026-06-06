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
        var userSkills = await GetUserSkillsAsync(userId, cancellationToken).ConfigureAwait(false);
        var groups = await skillGroupRepository.GetByJobRoleAsync(role, cancellationToken).ConfigureAwait(false);

        int totalWeight = 0;
        foreach (var group in groups)
            totalWeight += group.Weight;

        var groupScores = new List<double>();
        foreach (var group in groups)
            groupScores.Add(ComputeGroupScore(group, userSkills));

        double matchScore = ComputeMatchScore(groups, groupScores);

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

    public async Task<IReadOnlyList<RoleResult>> CalculateAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        var results = new List<RoleResult>();
        foreach (JobRole role in Enum.GetValues(typeof(JobRole)))
            results.Add(await CalculateForRoleAsync(userId, role, cancellationToken).ConfigureAwait(false));
        return results;
    }

    public IReadOnlyList<Suggestion> GetSuggestions(RoleResult result) => result.Suggestions;

    private async Task<List<CompatibilitySkill>> GetUserSkillsAsync(int userId, CancellationToken cancellationToken)
    {
        var profileSkills = await userSkillRepository.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        var user = await userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        var cvSkills = ExtractSkillsFromParsedCv(user?.ParsedCv ?? string.Empty);
        var skills = MergeProfileAndCvSkills(profileSkills, cvSkills);
        await ApplyBestTestAttemptScoresAsync(userId, skills, cancellationToken).ConfigureAwait(false);
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
            .Select(skillToAdjust => skillToAdjust.Trim())
            .Where(skillToAdjust => !string.IsNullOrWhiteSpace(skillToAdjust))
            .ToList();
    }

    private static List<CompatibilitySkill> MergeProfileAndCvSkills(IReadOnlyList<UserSkill> profileSkills, List<string> cvSkills)
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

    private async Task ApplyBestTestAttemptScoresAsync(int userId, List<CompatibilitySkill> skills, CancellationToken cancellationToken)
    {
        var completedAttempts = await testAttemptRepository
            .FindCompletedByUserIdAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        var bestAttempts = completedAttempts
            .Select(attempt => new
            {
                Attempt = attempt,
                Skill = attempt.Test?.Skill,
                Score = GetAttemptPercentageScore(attempt),
            })
            .Where(attemptInfo => attemptInfo.Skill is not null && attemptInfo.Score is not null)
            .GroupBy(attemptInfo => attemptInfo.Skill!.SkillId)
            .Select(group => group.OrderByDescending(attemptInfo => attemptInfo.Score).First());

        foreach (var bestAttempt in bestAttempts)
        {
            var skill = bestAttempt.Skill!;
            var score = NormalizeScore(bestAttempt.Score!.Value);
            var existing = skills.FirstOrDefault(profileSkill =>
                profileSkill.SkillId == skill.SkillId ||
                string.Equals(profileSkill.Name, skill.Name, StringComparison.OrdinalIgnoreCase));

            var source = $"Best test attempt: {bestAttempt.Attempt.Test?.Title ?? skill.Name}";
            if (existing is null)
            {
                // Only add test score if it's greater than the threshold
                if (score > TestScoreThreshold)
                {
                    skills.Add(new CompatibilitySkill(skill.SkillId, skill.Name, score, source));
                }
                continue;
            }

            // Only update score if test score is greater than the threshold
            if (score > TestScoreThreshold)
            {
                existing.SkillId ??= skill.SkillId;
                existing.Score = score;
                existing.Source = source;
            }
        }
    }

    private static double? GetAttemptPercentageScore(TestAttempt attempt)
    {
        if (attempt.PercentageScore.HasValue)
            return (double)attempt.PercentageScore.Value;

        var maximumPossibleScore = attempt.Test?.Questions.Sum(question => question.QuestionScore) ?? 0;
        if (!attempt.Score.HasValue || maximumPossibleScore <= 0)
            return null;

        return (double)attempt.Score.Value / maximumPossibleScore * ScoreNormalizationFactor;
    }

    private static double ComputeGroupScore(SkillGroup group, List<CompatibilitySkill> userSkills)
    {
        if (group.Skills.Count == 0)
            return 0;

        int totalSkillsInGroup = group.Skills.Count;
        double totalContribution = 0;

        // Each skill can contribute: (1 / totalSkillsInGroup) * (skillScore / 100)
        foreach (var skill in group.Skills)
        {
            var match = userSkills.FirstOrDefault(userSkill => SkillMatches(userSkill, skill));
            if (match is null)
            {
                // Skill not found, contributes 0
                continue;
            }

            // Each skill contributes: (skillScore / 100) / numberOfSkillsInGroup
            // Example: 20% baseline / 3 skills = 6.67% per skill
            double skillContribution = (match.Score / ScoreNormalizationFactor) / totalSkillsInGroup;
            totalContribution += skillContribution;
        }

        return totalContribution;
    }

    private static double ComputeMatchScore(IReadOnlyList<SkillGroup> groups, List<double> groupScores)
    {
        int totalWeight = groups.Sum(groupWithWeight => groupWithWeight.Weight);
        if (totalWeight == 0) return InvalidScore;
        double weighted = groups.Select((groupWithWeight, indexOfGroup) => groupWithWeight.Weight * groupScores[indexOfGroup]).Sum();
        return weighted * ScoreNormalizationFactor / totalWeight;
    }

    private static List<Suggestion> IdentifyGaps(IReadOnlyList<SkillGroup> skillGroups, List<CompatibilitySkill> userSkills, int totalWeight)
    {
        var suggestions = new List<Suggestion>();
        foreach (var group in skillGroups)
        {
            double groupScore = ComputeGroupScore(group, userSkills);
            if (groupScore > HighSkillCoverageThreshold) continue;

            var skill = group.Skills.FirstOrDefault(skillToAdjust => !userSkills.Any(userSkillWithPossibleSkill => SkillMatches(userSkillWithPossibleSkill, skillToAdjust)));
            if (skill is null) continue;

            suggestions.Add(new Suggestion
            {
                SkillName = skill.Name,
                GroupName = group.GroupName,
                GainScore = ScoreNormalizationFactor * group.Weight * (TargetGroupScore - groupScore) / totalWeight,
            });
        }

        return suggestions.OrderByDescending(suggestionsCheckScore => suggestionsCheckScore.GainScore).Take(MaxSuggestions).ToList();
    }

    private static List<CompatibilitySkillScore> BuildSkillScores(IReadOnlyList<SkillGroup> skillGroups, List<CompatibilitySkill> userSkills)
    {
        var roleSkills = skillGroups
            .SelectMany(group => group.Skills)
            .ToList();

        return userSkills
            .Where(userSkill => roleSkills.Any(roleSkill => SkillMatches(userSkill, roleSkill)))
            .OrderByDescending(userSkill => userSkill.Score)
            .ThenBy(userSkill => userSkill.Name)
            .Select(userSkill => new CompatibilitySkillScore
            {
                SkillName = userSkill.Name,
                Score = userSkill.Score,
                Source = userSkill.Source,
            })
            .ToList();
    }

    private static bool SkillMatches(CompatibilitySkill userSkill, Skill skill)
    {
        return userSkill.SkillId == skill.SkillId ||
               string.Equals(userSkill.Name, skill.Name, StringComparison.OrdinalIgnoreCase);
    }

    private static void AddOrImproveSkill(List<CompatibilitySkill> skills, CompatibilitySkill candidate)
    {
        var existing = skills.FirstOrDefault(skill =>
            string.Equals(skill.Name, candidate.Name, StringComparison.OrdinalIgnoreCase) ||
            (skill.SkillId is not null && skill.SkillId == candidate.SkillId));

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
