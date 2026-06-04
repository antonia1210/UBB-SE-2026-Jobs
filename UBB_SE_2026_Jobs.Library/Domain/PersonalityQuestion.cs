using UBB_SE_2026_Jobs.Library.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace UBB_SE_2026_Jobs.Library.Domain;

public class PersonalityQuestion
{
    [Key]
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = string.Empty;
    public TraitType Trait { get; set; }
    public int SortOrder { get; set; }
}
