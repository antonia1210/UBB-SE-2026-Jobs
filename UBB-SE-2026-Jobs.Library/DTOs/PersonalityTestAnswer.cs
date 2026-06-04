using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.DTOs;

public class PersonalityTestAnswer
{
    public string QuestionText { get; set; } = string.Empty;

    public TraitType Trait { get; set; }

    public int SortOrder { get; set; }

    public int Answer { get; set; }
}
