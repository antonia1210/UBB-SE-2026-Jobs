using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.Domain;

public class DeveloperPost
{
    public int DeveloperPostId { get; set; }
    public Developer Developer { get; set; } = null!;
    public DeveloperPostParameterType ParameterType { get; set; }
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
