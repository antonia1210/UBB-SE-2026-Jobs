namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class TestDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string QuestionTypeLabel { get; set; } = "MIXED";
}
