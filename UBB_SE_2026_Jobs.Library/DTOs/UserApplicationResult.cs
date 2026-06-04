using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.DTOs;

public class UserApplicationResult
{
    public required User User { get; set; }
    public required Match Match { get; set; }
    public required Job Job { get; set; }
    public double CompatibilityScore { get; set; }
    public IReadOnlyList<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    public string Feedback { get; set; } = string.Empty;
}
