using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Web.Models;

public class PersonalityTestViewModel
{
    public IReadOnlyList<PersonalityQuestion> Questions { get; set; } = [];
}

public class PersonalityTestSubmitModel
{
    // Key: QuestionText, Value: AnswerValue ca int (1-5)
    public Dictionary<string, int> Answers { get; set; } = [];
}

public class SelectRoleModel
{
    public Dictionary<string, int> Answers { get; set; } = [];
    public List<RoleOption> TopRoles { get; set; } = [];
}

public class RoleOption
{
    public JobRole Role { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public double Score { get; set; }
}
