using UBB_SE_2026_Jobs.Web.Dtos;

namespace UBB_SE_2026_Jobs.Web.Models
{
    public class LeaderboardViewModel
    {
        public int TestId { get; set; }

        public List<LeaderboardEntryDto> Entries { get; set; } = new();

        public LeaderboardEntryDto? CurrentUserEntry { get; set; }

        public List<LeaderboardEntryDto> TopEntries =>
            this.Entries
                .OrderBy(entry => entry.RankPosition)
                .Take(3)
                .ToList();
    }
}
