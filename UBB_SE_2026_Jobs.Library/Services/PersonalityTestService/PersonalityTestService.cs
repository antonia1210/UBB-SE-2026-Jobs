using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Repositories.PersonalityTests;

namespace UBB_SE_2026_Jobs.Library.Services.PersonalityTestService;

public class PersonalityTestService : IPersonalityTestService
{
    private const int FrontendVisibilityWeight = 2;
    private const int FrontendCreativityWeight = 2;

    private const int BackendDepthWeight = 2;
    private const int BackendVisibilityWeight = 2;
    private const int BackendVisibilityBaseline = 5;

    private const int UiUxVisibilityWeight = 3;
    private const int UiUxCreativityWeight = 2;

    private const int DevOpsDepthWeight = 2;
    private const int DevOpsPaceWeight = 2;
    private const int DevOpsInteractionBaseline = 5;

    private const int ProjectManagerInteractionWeight = 3;
    private const int ProjectManagerDepthBaseline = 5;

    private const int DataAnalystDepthWeight = 2;
    private const int DataAnalystAbstractionWeight = 2;
    private const int DataAnalystInteractionBaseline = 5;

    private const int CybersecurityDepthWeight = 3;
    private const int CybersecurityInteractionBaseline = 6;
    private const int CybersecurityPaceBaseline = 6;

    private const int AiEngineerDepthWeight = 3;
    private const int AiEngineerAbstractionWeight = 2;

    private readonly IPersonalityTestRepository personalityTestRepository;

    public PersonalityTestService(IPersonalityTestRepository personalityTestRepository)
    {
        this.personalityTestRepository = personalityTestRepository;
    }

    public static IReadOnlyList<PersonalityQuestion> LoadQuestions()
    {
        int sortOrder = 0;
        PersonalityQuestion Make(string text, TraitType trait) =>
            new PersonalityQuestion { QuestionText = text, Trait = trait, SortOrder = ++sortOrder };

        var questions = new List<PersonalityQuestion>();
        questions.AddRange(GetVisibilityTraitQuestions(Make));
        questions.AddRange(GetInteractionTraitQuestions(Make));
        questions.AddRange(GetDepthTraitQuestions(Make));
        questions.AddRange(GetCreativityTraitQuestions(Make));
        questions.AddRange(GetPaceTraitQuestions(Make));
        questions.AddRange(GetAbstractionTraitQuestions(Make));
        return questions.AsReadOnly();
    }

    public async Task<PersonalityTestResult?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await personalityTestRepository.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

    private static List<PersonalityQuestion> GetVisibilityTraitQuestions(Func<string, TraitType, PersonalityQuestion> make)
    {
        return
        [
            make("I notice design details in apps and websites that most people would overlook.", TraitType.Visibility),
            make("I believe a seamless, high-quality user interface is just as critical to a project's success as the underlying code.", TraitType.Visibility),
            make("I believe that a project isn't truly 'finished' until the visual polish matches the technical quality.", TraitType.Visibility),
            make("I find myself drawn to tools and interfaces that are clean and well-designed.", TraitType.Visibility),
        ];
    }

    private static List<PersonalityQuestion> GetInteractionTraitQuestions(Func<string, TraitType, PersonalityQuestion> make)
    {
        return
        [
            make("I enjoy collaborating with others more than working through problems on my own.", TraitType.Interaction),
            make("I feel energized after meetings or group discussions rather than drained.", TraitType.Interaction),
            make("I would rather manage relationships and expectations than debug a technical issue.", TraitType.Interaction),
            make("I prefer roles where communication is a big part of the daily work.", TraitType.Interaction),
        ];
    }

    private static List<PersonalityQuestion> GetDepthTraitQuestions(Func<string, TraitType, PersonalityQuestion> make)
    {
        return
        [
            make("When something breaks, I want to understand exactly why — not just fix the surface issue.", TraitType.Depth),
            make("I enjoy reading documentation or technical material to fully understand a system.", TraitType.Depth),
            make("I find it satisfying to deeply master one topic rather than know a little about many.", TraitType.Depth),
            make("I get curious about what's happening \"behind the scenes\" in the tools and systems I use.", TraitType.Depth),
        ];
    }

    private static List<PersonalityQuestion> GetCreativityTraitQuestions(Func<string, TraitType, PersonalityQuestion> make)
    {
        return
        [
            make("I thrive when given a problem with no clear solution rather than a checklist to follow.", TraitType.Creativity),
            make("I enjoy coming up with new ideas more than executing someone else's plan.", TraitType.Creativity),
            make("I prefer work that leaves room for experimentation over work with strict rules and procedures.", TraitType.Creativity),
            make("I am most productive when tackling new problems rather than refining existing processes.", TraitType.Creativity),
        ];
    }

    private static List<PersonalityQuestion> GetPaceTraitQuestions(Func<string, TraitType, PersonalityQuestion> make)
    {
        return
        [
            make("I work best when I have several different tasks to switch between throughout the day.", TraitType.Pace),
            make("I enjoy fast-paced environments where priorities shift and I have to adapt quickly.", TraitType.Pace),
            make("I prefer having many smaller responsibilities over owning one large long-term problem.", TraitType.Pace),
            make("I feel productive when I can check off multiple different things in a single day.", TraitType.Pace),
        ];
    }

    private static List<PersonalityQuestion> GetAbstractionTraitQuestions(Func<string, TraitType, PersonalityQuestion> make)
    {
        return
        [
            make("I enjoy working with mathematical concepts, formulas, or statistical models.", TraitType.Abstraction),
            make("I find theoretical or abstract problems more interesting than purely practical ones.", TraitType.Abstraction),
            make("I am comfortable working with data, probabilities, and logical frameworks.", TraitType.Abstraction),
            make("I prefer to understand the logic and first principles of a system rather than just knowing how to operate it.", TraitType.Abstraction),
        ];
    }

    public IReadOnlyDictionary<TraitType, double> CalculateTraitScores(IReadOnlyDictionary<PersonalityQuestion, AnswerValue> personalityTestAnswers)
    {
        var totalScorePerTrait = new Dictionary<TraitType, double>();
        var questionCountPerTrait = new Dictionary<TraitType, int>();

        foreach (var personalityTestAnswer in personalityTestAnswers)
        {
            var questionTrait = personalityTestAnswer.Key.Trait;

            if (!totalScorePerTrait.ContainsKey(questionTrait))
            {
                totalScorePerTrait[questionTrait] = 0;
                questionCountPerTrait[questionTrait] = 0;
            }

            totalScorePerTrait[questionTrait] += (int)personalityTestAnswer.Value;
            questionCountPerTrait[questionTrait]++;
        }

        foreach (var trait in totalScorePerTrait.Keys)
        {
            totalScorePerTrait[trait] /= questionCountPerTrait[trait];
        }

        return totalScorePerTrait;
    }

    public IReadOnlyDictionary<JobRole, double> CalculateRoleScores(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        var roleScores = new Dictionary<JobRole, double>();
        roleScores.Add(JobRole.FrontendDeveloper, CalculateFrontend(traitScores));
        roleScores.Add(JobRole.BackendDeveloper, CalculateBackend(traitScores));
        roleScores.Add(JobRole.UiUxDesigner, CalculateUiUxDesigner(traitScores));
        roleScores.Add(JobRole.DevOpsEngineer, CalculateDevOps(traitScores));
        roleScores.Add(JobRole.ProjectManager, CalculateProjectManager(traitScores));
        roleScores.Add(JobRole.DataAnalyst, CalculateDataAnalyst(traitScores));
        roleScores.Add(JobRole.CybersecuritySpecialist, CalculateCybersecurity(traitScores));
        roleScores.Add(JobRole.AiMlEngineer, CalculateAiMlEngineer(traitScores));
        return roleScores;
    }

    public IReadOnlyDictionary<JobRole, double> GetTopRoles(IReadOnlyDictionary<JobRole, double> roleScores, int count)
    {
        return roleScores
            .OrderByDescending(roleWithScore => roleWithScore.Value)
            .Take(count)
            .ToDictionary(roleWithScore => roleWithScore.Key, roleWithScore => roleWithScore.Value);
    }

    public async Task SaveResultAsync(int userId, IReadOnlyDictionary<PersonalityQuestion, AnswerValue> answers, JobRole selectedRole, CancellationToken cancellationToken = default)
    {
        var traitScores = CalculateTraitScores(answers);

        var traitScoreEntities = traitScores
            .Select(traitWithScore => new PersonalityTraitScore
            {
                Trait = traitWithScore.Key,
                Score = (int)Math.Round(traitWithScore.Value),
            })
            .ToList();

        var newResult = new PersonalityTestResult
        {
            User = new User { UserId = userId },
            CompletedAt = DateTime.UtcNow,
            SelectedRole = selectedRole,
            TraitScores = traitScoreEntities,
        };

        var existingResult = await personalityTestRepository.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (existingResult is null)
        {
            await personalityTestRepository.AddAsync(newResult, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            newResult.PersonalityTestResultId = existingResult.PersonalityTestResultId;
            await personalityTestRepository.UpdateAsync(newResult, cancellationToken).ConfigureAwait(false);
        }
    }

    private double CalculateFrontend(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        return (traitScores[TraitType.Visibility] * FrontendVisibilityWeight) +
               (traitScores[TraitType.Creativity] * FrontendCreativityWeight) +
               traitScores[TraitType.Pace];
    }

    private double CalculateBackend(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        return (traitScores[TraitType.Depth] * BackendDepthWeight) +
               ((BackendVisibilityBaseline - traitScores[TraitType.Visibility]) * BackendVisibilityWeight) +
               traitScores[TraitType.Pace];
    }

    private double CalculateUiUxDesigner(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        return (traitScores[TraitType.Visibility] * UiUxVisibilityWeight) +
               (traitScores[TraitType.Creativity] * UiUxCreativityWeight) +
               traitScores[TraitType.Interaction];
    }

    private double CalculateDevOps(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        return (traitScores[TraitType.Depth] * DevOpsDepthWeight) +
               (traitScores[TraitType.Pace] * DevOpsPaceWeight) +
               (DevOpsInteractionBaseline - traitScores[TraitType.Interaction]);
    }

    private double CalculateProjectManager(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        return (traitScores[TraitType.Interaction] * ProjectManagerInteractionWeight) +
               traitScores[TraitType.Creativity] +
               (ProjectManagerDepthBaseline - traitScores[TraitType.Depth]);
    }

    private double CalculateDataAnalyst(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        return (traitScores[TraitType.Depth] * DataAnalystDepthWeight) +
               (traitScores[TraitType.Abstraction] * DataAnalystAbstractionWeight) +
               (DataAnalystInteractionBaseline - traitScores[TraitType.Interaction]);
    }

    private double CalculateCybersecurity(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        return (traitScores[TraitType.Depth] * CybersecurityDepthWeight) +
               (CybersecurityInteractionBaseline - traitScores[TraitType.Interaction]) +
               (CybersecurityPaceBaseline - traitScores[TraitType.Pace]);
    }

    private double CalculateAiMlEngineer(IReadOnlyDictionary<TraitType, double> traitScores)
    {
        return (traitScores[TraitType.Depth] * AiEngineerDepthWeight) +
               traitScores[TraitType.Creativity] +
               (traitScores[TraitType.Abstraction] * AiEngineerAbstractionWeight);
    }
}