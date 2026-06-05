namespace UBB_SE_2026_Jobs.Library.DTOs;

public sealed class UserMatchmakingFilters
{
    public HashSet<string> EmploymentTypes { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> ExperienceLevels { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> WorkModes { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public string LocationSubstring { get; set; } = string.Empty;
    public HashSet<int> SkillIds { get; init; } = new();

    public static UserMatchmakingFilters Empty() => new();
}
