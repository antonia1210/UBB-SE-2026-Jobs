using System.Text.Json.Serialization;

namespace UBB_SE_2026_Jobs.Library.Domain;

public class JobSkill
{
    public Job Job { get; set; } = null!;

    public Skill Skill { get; set; } = null!;

    public int RequiredLevel { get; set; }
}
