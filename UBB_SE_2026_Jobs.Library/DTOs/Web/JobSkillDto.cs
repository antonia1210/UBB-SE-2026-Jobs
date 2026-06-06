namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class JobSkillDto
{
    public int SkillId { get; set; }
    public int JobId { get; set; }
    public int RequiredPercentage { get; set; }
    public SkillDto SkillDto { get; set; } = null!;
}
