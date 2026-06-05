using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Services.TI;
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
            // Two calls total instead of N+1
            var attempts = await tiTestService.GetAttemptsByUserAsync(userId);
            var attemptByTestId = attempts.ToDictionary(a => a.TestId);

            // Only fetch tests the user has actually attempted
            var cards = new List<SkillTestCardViewModel>();
            foreach (var attempt in attempts)
            {
                var test = await tiTestService.GetByIdAsync(attempt.TestId);
                if (test is null) continue;

                var questions = await tiTestService.GetQuestionsByTestIdAsync(test.Id);
                float maxPossibleScore = questions.Sum(q => q.QuestionScore);

                cards.Add(new SkillTestCardViewModel(test, attempt, maxPossibleScore));
            }

            TestCards = cards;
            ErrorMessage = cards.Count == 0 ? "No completed skill tests yet." : null;
        }
        catch (Exception exception)
        {
            TestCards = new List<SkillTestCardViewModel>();
            ErrorMessage = $"Couldn't load skill tests. ({exception.Message})";
        }
    }
}
