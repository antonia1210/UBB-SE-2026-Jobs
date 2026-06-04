using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.Domain;

public class SkillGroup
{
    public int SkillGroupId { get; set; }

    public string GroupName { get; set; } = string.Empty;
    public int Weight { get; set; }
    public JobRole JobRole { get; set; }

    public List<Skill> Skills { get; set; } = new();
}
