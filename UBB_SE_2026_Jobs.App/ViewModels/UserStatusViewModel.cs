using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;

namespace UBB_SE_2026_Jobs.App.ViewModels;

public class UserStatusViewModel : DispatchableObservableObject
{
    private readonly IUserStatusService userStatusService;
    private readonly SessionContext session;
    private bool isLoading;
    private bool hasError;
    private bool isEmpty;
    private bool showCards;
    private string emptyMessage = string.Empty;
    private string currentFilter = "All";
    private bool showGoToRecommendations;

    public UserStatusViewModel(
        IUserStatusService userStatusService,
        SessionContext session)
    {
        this.userStatusService = userStatusService;
        this.session = session;

        RefreshCommand = new RelayCommand(Refresh);
    }

    public ObservableCollection<ApplicationCardModel> AppliedJobs { get; } = new();
    public ObservableCollection<ApplicationCardModel> FilteredJobs { get; } = new();

    public bool IsLoading { get => isLoading; set => SetProperty(ref isLoading, value); }
    public bool HasError { get => hasError; set => SetProperty(ref hasError, value); }
    public bool IsEmpty { get => isEmpty; set => SetProperty(ref isEmpty, value); }
    public bool ShowCards { get => showCards; set => SetProperty(ref showCards, value); }
    public bool ShowGoToRecommendations { get => showGoToRecommendations; set => SetProperty(ref showGoToRecommendations, value); }
    public string EmptyMessage { get => emptyMessage; set => SetProperty(ref emptyMessage, value); }
    public string CurrentFilter { get => currentFilter; set => SetProperty(ref currentFilter, value); }
    public ICommand RefreshCommand { get; }

    public async Task LoadMatchesAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        HasError = false;
        IsEmpty = false;
        ShowCards = false;

        var userId = ViewModelSupport.ResolveUserId(session);

        try
        {
            var applications = await userStatusService.GetApplicationsForUserAsync(userId, cancellationToken);

            await UIDispatcher.EnqueueAsync(() =>
            {
                AppliedJobs.Clear();
                foreach (var application in applications)
                {
                    AppliedJobs.Add(application);
                }

                ApplyFilter(CurrentFilter);
            });
        }
        catch
        {
            HasError = true;
            ShowCards = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Refresh()
    {
        UIDispatcher.Enqueue(() =>
        {
            AppliedJobs.Clear();
            FilteredJobs.Clear();
        });
        _ = LoadMatchesAsync();
    }

    public void ApplyFilter(string filter)
    {
        CurrentFilter = filter;
        FilteredJobs.Clear();

        foreach (var application in GetFilteredApplications(filter))
        {
            FilteredJobs.Add(application);
        }

        if (FilteredJobs.Count == 0)
        {
            IsEmpty = true;
            ShowCards = false;
            EmptyMessage = AppliedJobs.Count == 0
                ? "You have not applied to any jobs yet. Head to the Recommendations page to get started."
                : "No applications match this filter.";
            ShowGoToRecommendations = AppliedJobs.Count == 0;
        }
        else
        {
            IsEmpty = false;
            ShowCards = true;
            ShowGoToRecommendations = false;
        }
    }

    private IEnumerable<ApplicationCardModel> GetFilteredApplications(string filter)
    {
        return filter switch
        {
            "Applied" => GetApplicationsByStatus(MatchStatus.Applied),
            "Accepted" => GetApplicationsByStatus(MatchStatus.Accepted),
            "Rejected" => GetApplicationsByStatus(MatchStatus.Rejected),
            _ => AppliedJobs,
        };
    }

    private IEnumerable<ApplicationCardModel> GetApplicationsByStatus(MatchStatus status)
    {
        return AppliedJobs.Where(application => application.Status == status).ToList();
    }
}
