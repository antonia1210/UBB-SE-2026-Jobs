using UBB_SE_2026_Jobs.Library.Domain.Enums;
using System.Text.Json.Serialization;

namespace UBB_SE_2026_Jobs.Library.Domain;

public class PersonalityTestResult
{
    public int PersonalityTestResultId { get; set; }

    public User User { get; set; } = null!;

    public DateTime CompletedAt { get; set; }

    public JobRole? SelectedRole { get; set; }

    public List<PersonalityTraitScore> TraitScores { get; set; } = new();
}
