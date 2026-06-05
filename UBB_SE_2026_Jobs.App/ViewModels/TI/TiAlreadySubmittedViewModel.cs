using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Services.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiAlreadySubmittedViewModel : DispatchableObservableObject
{
    private readonly ITiTestService testService;
    private readonly SessionContext session;

    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private int attemptId;
    [ObservableProperty] private bool hasAttempt;

    public TiAlreadySubmittedViewModel(ITiTestService testService, SessionContext session)
    {
        this.testService = testService;
        this.session = session;
    }

    public async Task LoadAsync(int testId)
    {
        IsLoading = true;
        var attempt = await testService.GetAttemptByUserAndTestAsync(session.UserId, testId);
        if (attempt != null)
        {
            AttemptId = attempt.Id;
            HasAttempt = true;
        }
        IsLoading = false;
    }
}
