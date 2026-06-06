using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiRecruiterInterviewsViewModel : DispatchableObservableObject
{
    private readonly ITiInterviewSessionService sessionService;
    private readonly SessionContext session;

    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private Visibility isEmpty = Visibility.Collapsed;
    [ObservableProperty] private Visibility hasSessions = Visibility.Collapsed;

    public ObservableCollection<TiInterviewSessionDto> BookedSessions { get; } = new();

    public TiRecruiterInterviewsViewModel(
        ITiInterviewSessionService sessionService,
        SessionContext session)
    {
        this.sessionService = sessionService;
        this.session = session;
    }

    public async Task LoadAllAsync()
    {
        IsLoading = true;
        var sessions = await sessionService.GetScheduledAsync(session.UserId);
        await UIDispatcher.EnqueueAsync(() =>
        {
            BookedSessions.Clear();
            foreach (var s in sessions)
                BookedSessions.Add(s);

            IsEmpty = BookedSessions.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            HasSessions = BookedSessions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        });
        IsLoading = false;
    }

    public async Task SubmitDecisionAsync(int sessionId, string decision)
    {
        await sessionService.SetInterviewDecisionAsync(sessionId, decision);
        await LoadAllAsync();
    }
}