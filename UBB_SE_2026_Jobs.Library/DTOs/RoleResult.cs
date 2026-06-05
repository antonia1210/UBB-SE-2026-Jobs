using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.DTOs;

public class RoleResult
{
    public JobRole JobRole { get; set; }
    public double MatchScore { get; set; }
    public List<Suggestion> Suggestions { get; set; } = new();
    public List<CompatibilitySkillScore> SkillScores { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class CompatibilitySkillScore
{
    public string SkillName { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Source { get; set; } = string.Empty;
}
