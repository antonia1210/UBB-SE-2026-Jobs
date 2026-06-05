namespace UBB_SE_2026_Jobs.App.Dtos.TI;

/// <summary>
/// DTO for deserializing applications from the API.
/// Uses string for Status to handle flexible JSON deserialization.
/// </summary>
public class TiApplicationDto
{
    public int MatchId { get; set; }
    public int JobId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public DateTime AppliedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CompatibilityScore { get; set; }
    public string FeedbackMessage { get; set; } = string.Empty;

    public string TruncatedDescription =>
        JobDescription.Length > 120 ? JobDescription[..120] + "..." : JobDescription;

    public string FormattedDate => $"Applied on {AppliedDate:dd MMM yyyy}";
    public string FormattedScore => $"{CompatibilityScore}% match";
    public bool HasFeedback => !string.IsNullOrEmpty(FeedbackMessage);
}
