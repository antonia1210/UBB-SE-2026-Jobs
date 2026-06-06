using System.Text.Json;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

internal static class TestCatalogSeed
{
    private static readonly DateTime SeedCreatedAt = new(2026, 1, 1);
    private const int QuestionsPerTest = 5;
    private const float QuestionScore = 10f;

    private static readonly IReadOnlyDictionary<int, int[]> SkillIdsByGroup = new Dictionary<int, int[]>
    {
        [1] = [33, 34, 35, 36],
        [2] = [37, 31],
        [3] = [2, 29, 30, 38],
        [4] = [39, 40],
        [5] = [41, 42, 5],
        [6] = [43, 44, 45],
        [7] = [12, 46, 47],
        [8] = [21, 1, 8, 48, 25],
        [9] = [22, 49, 50],
        [10] = [3, 26, 51, 52, 53],
        [11] = [54, 55, 56],
        [12] = [39, 40],
        [13] = [57, 58, 59, 60],
        [14] = [12, 46, 61, 62],
        [15] = [63, 64, 65],
        [16] = [66, 67, 68],
        [17] = [69, 70, 71],
        [18] = [47, 12, 72],
        [19] = [73, 74, 75],
        [20] = [6, 76],
        [21] = [7, 77, 78],
        [22] = [79, 80, 81, 82],
        [23] = [32, 83, 84],
        [24] = [85, 86, 87],
        [25] = [88, 89, 90],
        [26] = [91, 92, 23, 93],
        [27] = [94, 95, 96],
        [28] = [97, 98],
        [29] = [99, 100, 101],
        [30] = [102, 103, 104],
        [31] = [3, 26, 105],
        [32] = [106, 107, 108],
        [33] = [8, 109],
        [34] = [110, 111, 112],
        [35] = [113, 114],
        [36] = [9, 115],
        [37] = [116, 117, 118, 119],
        [38] = [120, 121, 122],
        [39] = [123, 124, 125],
        [40] = [126, 127, 128, 129],
        [41] = [130, 131, 132, 133],
        [42] = [134, 135, 136],
        [43] = [137, 28, 138, 139],
        [44] = [8, 109, 140],
        [45] = [141, 142, 143, 144],
        [46] = [9, 145, 146, 3],
        [47] = [6, 147, 148],
        [48] = [149, 150, 151, 152],
    };

    public static readonly Test[] Tests = BuildTests();

    public static readonly TestQuestion[] Questions = BuildQuestions();

    private static Test[] BuildTests()
    {
        return SkillGroupSeed.Groups
            .OrderBy(group => group.SkillGroupId)
            .Select(group => new Test
            {
                Id = group.SkillGroupId,
                Title = $"{DisplayRole(group.JobRole)} - {group.GroupName}",
                Category = DisplayRole(group.JobRole),
                CreatedAt = SeedCreatedAt,
                SkillId = SkillIdsByGroup[group.SkillGroupId][0],
            })
            .ToArray();
    }

    private static TestQuestion[] BuildQuestions()
    {
        var skillsById = SkillCatalog.Seed.ToDictionary(skill => skill.SkillId, skill => skill.Name);
        var questions = new List<TestQuestion>();

        foreach (var group in SkillGroupSeed.Groups.OrderBy(group => group.SkillGroupId))
        {
            var skillNames = SkillIdsByGroup[group.SkillGroupId]
                .Select(skillId => skillsById[skillId])
                .ToArray();

            string role = DisplayRole(group.JobRole);
            string primarySkill = skillNames[0];
            string secondarySkill = skillNames.Length > 1 ? skillNames[1] : skillNames[0];
            string[] wrongSkills = BuildWrongSkillOptions(skillNames, skillsById.Values);
            int questionBaseId = (group.SkillGroupId - 1) * QuestionsPerTest;

            questions.Add(new TestQuestion
            {
                Id = questionBaseId + 1,
                TestId = group.SkillGroupId,
                QuestionText = $"Which skill best fits the {group.GroupName} group for {role}?",
                QuestionTypeString = "SINGLE_CHOICE",
                QuestionScore = QuestionScore,
                QuestionAnswer = "0",
                OptionsJson = Options(primarySkill, wrongSkills[0], wrongSkills[1], wrongSkills[2]),
            });

            questions.Add(new TestQuestion
            {
                Id = questionBaseId + 2,
                TestId = group.SkillGroupId,
                QuestionText = $"Select the two skills that belong to {group.GroupName}.",
                QuestionTypeString = "MULTIPLE_CHOICE",
                QuestionScore = QuestionScore,
                QuestionAnswer = "[0,1]",
                OptionsJson = Options(primarySkill, secondarySkill, wrongSkills[0], wrongSkills[1]),
            });

            questions.Add(new TestQuestion
            {
                Id = questionBaseId + 3,
                TestId = group.SkillGroupId,
                QuestionText = $"{primarySkill} is part of the {group.GroupName} skill group.",
                QuestionTypeString = "TRUE_FALSE",
                QuestionScore = QuestionScore,
                QuestionAnswer = "true",
                OptionsJson = null,
            });

            questions.Add(new TestQuestion
            {
                Id = questionBaseId + 4,
                TestId = group.SkillGroupId,
                QuestionText = $"Type the primary skill tested for {group.GroupName}.",
                QuestionTypeString = "TEXT",
                QuestionScore = QuestionScore,
                QuestionAnswer = primarySkill,
                OptionsJson = null,
            });

            questions.Add(new TestQuestion
            {
                Id = questionBaseId + 5,
                TestId = group.SkillGroupId,
                QuestionText = $"Type the exact skill group name for this {role} test.",
                QuestionTypeString = "TEXT",
                QuestionScore = QuestionScore,
                QuestionAnswer = group.GroupName,
                OptionsJson = null,
            });
        }

        return questions.ToArray();
    }

    private static string[] BuildWrongSkillOptions(IReadOnlyCollection<string> correctSkills, IEnumerable<string> allSkillNames)
    {
        return allSkillNames
            .Where(skillName => !correctSkills.Contains(skillName, StringComparer.OrdinalIgnoreCase))
            .Take(3)
            .ToArray();
    }

    private static string Options(params string[] options) => JsonSerializer.Serialize(options);

    private static string DisplayRole(JobRole role) => role switch
    {
        JobRole.FrontendDeveloper => "Frontend Developer",
        JobRole.BackendDeveloper => "Backend Developer",
        JobRole.UiUxDesigner => "UI/UX Designer",
        JobRole.DevOpsEngineer => "DevOps Engineer",
        JobRole.ProjectManager => "Project Manager",
        JobRole.DataAnalyst => "Data Analyst",
        JobRole.CybersecuritySpecialist => "Cybersecurity Specialist",
        JobRole.AiMlEngineer => "AI/ML Engineer",
        _ => role.ToString(),
    };
}
