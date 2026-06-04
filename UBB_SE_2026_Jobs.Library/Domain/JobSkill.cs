using System.Text.Json.Serialization;

namespace UBB_SE_2026_Jobs.Library.Domain;

public class JobSkill
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    public int RequiredLevel { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public int RequiredPercentage { get => RequiredLevel; set => RequiredLevel = value; }
}
