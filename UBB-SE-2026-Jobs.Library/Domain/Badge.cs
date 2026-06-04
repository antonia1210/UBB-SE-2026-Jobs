using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.Domain;

public class Badge
{
    public int BadgeId { get; set; }

    public BadgeTier Tier { get; set; }
    public string IconPath { get; set; } = string.Empty;
    public int ExperiencePointsValue { get; set; }
}
