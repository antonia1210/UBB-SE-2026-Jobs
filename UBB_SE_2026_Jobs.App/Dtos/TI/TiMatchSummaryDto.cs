namespace UBB_SE_2026_Jobs.App.Dtos.TI;

public class TiMatchSummaryDto
{
    public int MatchId { get; set; }
    public int UserId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public string FeedbackMessage { get; set; } = string.Empty;
}
