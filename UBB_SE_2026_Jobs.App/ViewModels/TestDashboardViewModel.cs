using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.Library.ServiceProxies.TI;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.App.ViewModels;

/// <summary>
/// Backs the desktop "Skill Tests" tab. Sources the user's real test results from the TI
/// engine (<see cref="ITiTestService"/>) rather than the PussyCats SkillTests table — each
/// card is a TI test plus the user's attempt (status + real percentage score).
/// </summary>
public class TestDashboardViewModel : DispatchableObservableObject
{
    private readonly ITiTestService tiTestService;
    private readonly SessionContext session;

    private List<SkillTestCardViewModel> testCards = new();
    private string? errorMessage;

    public TestDashboardViewModel(ITiTestService tiTestService, SessionContext session)
    {
        this.tiTestService = tiTestService;
        this.session = session;
    }

    public List<SkillTestCardViewModel> TestCards
    {
        get => testCards;
        private set => SetProperty(ref testCards, value);
    }

    public string? ErrorMessage
    {
        get => errorMessage;
        private set => SetProperty(ref errorMessage, value);
    }

    public async Task LoadTestsAsync(User? user = null, CancellationToken cancellationToken = default)
    {
        var userId = user?.UserId > 0 ? user.UserId : ViewModelSupport.ResolveUserId(session);

        try
        {
            var attempts = await tiTestService.GetAttemptsByUserAsync(userId);

            // Build one card per attempt (multiple attempts per test are shown individually).
            var cards = new List<SkillTestCardViewModel>();
            var testCache = new Dictionary<int, TiTestDto?>();
            var questionCountCache = new Dictionary<int, float>();

            foreach (var attempt in attempts)
            {
                if (!testCache.TryGetValue(attempt.TestId, out var test))
                {
                    test = await tiTestService.GetByIdAsync(attempt.TestId);
                    testCache[attempt.TestId] = test;
                }
                if (test is null) continue;

                if (!questionCountCache.TryGetValue(attempt.TestId, out float maxScore))
                {
                    var questions = await tiTestService.GetQuestionsByTestIdAsync(test.Id);
                    maxScore = questions.Sum(question => question.QuestionScore);
                    questionCountCache[attempt.TestId] = maxScore;
                }

                cards.Add(new SkillTestCardViewModel(test, attempt, maxScore));
            }

            TestCards = cards;
            ErrorMessage = cards.Count == 0 ? "No completed test attempts yet." : null;
        }
        catch (Exception exception)
        {
            TestCards = new List<SkillTestCardViewModel>();
            ErrorMessage = $"Couldn't load skill tests. ({exception.Message})";
        }
    }
}
