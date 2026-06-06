namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class TestAttemptDto
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int? ExternalUserId { get; set; }
    public decimal? Score { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string AnswersFilePath { get; set; } = string.Empty;
    public bool IsValidated { get; set; }
    public decimal? PercentageScore { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? RejectedAt { get; set; }
    public List<AnswerDto>? Answers { get; set; }
}
