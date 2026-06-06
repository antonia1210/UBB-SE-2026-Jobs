namespace UBB_SE_2026_Jobs.Library.DTOs.TI;

public class TiAnswerDto
{
    public int QuestionId { get; set; }
    public string Value { get; set; } = string.Empty;
    public int AttemptId { get; set; }
    public TiQuestionDto? Question { get; set; }
}
