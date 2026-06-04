using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.Domain;

public class DeveloperInteraction
{
    public int DeveloperInteractionId { get; set; }
    public Developer Developer { get; set; }
    public DeveloperPost DeveloperPost { get; set; }
    public DeveloperInteractionType Type { get; set; }
}
