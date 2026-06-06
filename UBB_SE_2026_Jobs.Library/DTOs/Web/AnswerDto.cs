namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class AnswerDto
{
    public int QuestionId { get; set; }
    public string Value { get; set; } = string.Empty;
    public int AttemptId { get; set; }
    public QuestionDto? Question { get; set; }
}
