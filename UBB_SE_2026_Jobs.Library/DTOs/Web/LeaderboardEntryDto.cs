namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class LeaderboardEntryDto
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int UserId { get; set; }
    public decimal NormalizedScore { get; set; }
    public int RankPosition { get; set; }
    public int TieBreakPriority { get; set; }
    public DateTime LastRecalculationAt { get; set; }
}
