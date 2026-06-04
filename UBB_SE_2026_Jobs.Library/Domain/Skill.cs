namespace UBB_SE_2026_Jobs.Library.Domain;

public class Skill
{
    public int SkillId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string SkillName { get => Name; set => Name = value; }
    public string Category { get; set; } = string.Empty;
}
