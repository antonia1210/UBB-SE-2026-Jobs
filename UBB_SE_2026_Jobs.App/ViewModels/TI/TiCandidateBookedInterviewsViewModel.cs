using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiCandidateBookedInterviewsViewModel : DispatchableObservableObject
{
    private readonly ITiSlotsService slotsService;
    private readonly SessionContext session;

    [ObservableProperty] private bool isLoading = false;
    [ObservableProperty] private bool isEmpty = false;
    [ObservableProperty] private string errorMessage = string.Empty;

    public ObservableCollection<TiInterviewSessionDto> BookedInterviews { get; } = new();

    public TiCandidateBookedInterviewsViewModel(ITiSlotsService slotsService, SessionContext session)
    {
        this.slotsService = slotsService;
        this.session = session;
    }

    public async Task LoadBookedInterviewsAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        IsEmpty = false;

        try
        {
            var interviews = await slotsService.GetBookedInterviewsByCandidate(session.UserId);

            await UIDispatcher.EnqueueAsync(() =>
            {
                BookedInterviews.Clear();
                foreach (var interview in interviews)
                {
                    BookedInterviews.Add(interview);
                }
                IsEmpty = BookedInterviews.Count == 0;
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not load booked interviews: {ex.Message}";
            IsEmpty = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void StartInterview(TiInterviewSessionDto interview)
    {
        ErrorMessage = "Interview recording is not available yet.";
    }

    [RelayCommand]
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        await LoadBookedInterviewsAsync(cancellationToken);
    }
}
