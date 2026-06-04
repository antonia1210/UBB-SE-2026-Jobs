using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.DTOs;

public class RoleResult
{
    public JobRole JobRole { get; set; }
    public double MatchScore { get; set; }
    public List<Suggestion> Suggestions { get; set; } = new();
}
