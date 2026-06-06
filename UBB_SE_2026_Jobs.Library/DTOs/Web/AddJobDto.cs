namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class AddJobDto
{
    public JobPostingDto Job { get; set; } = null!;
    public int UserId { get; set; }
    public List<JobSkillDto> SkillLinks { get; set; } = new();
}
