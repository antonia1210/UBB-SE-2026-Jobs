using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.Library.ServiceProxies.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiEventsViewModel : DispatchableObservableObject
{
    private readonly ITiEventsService eventsService;
    private readonly SessionContext session;

    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private bool showPastEvents;

    public ObservableCollection<TiEventDto> CurrentEvents { get; } = new();
    public ObservableCollection<TiEventDto> PastEvents { get; } = new();

    public TiEventsViewModel(ITiEventsService eventsService, SessionContext session)
    {
        this.eventsService = eventsService;
        this.session = session;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        int companyId = session.CompanyId ?? 1;

        var current = await eventsService.GetCurrentEventsAsync(companyId);
        var past = await eventsService.GetPastEventsAsync(companyId);

        CurrentEvents.Clear();
        foreach (var currentEvent in current) CurrentEvents.Add(currentEvent);

        PastEvents.Clear();
        foreach (var pastEvent in past) PastEvents.Add(pastEvent);

        IsLoading = false;
    }

    public async Task DeleteEventAsync(int eventId)
    {
        await eventsService.DeleteAsync(eventId);
        await LoadAsync();
    }
}
