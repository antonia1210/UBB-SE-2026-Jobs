using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiEventDetailsViewModel : DispatchableObservableObject
{
    private readonly SessionContext session;
    private readonly ITiEventsService eventsService;

    [ObservableProperty] private TiEventDto? currentEvent;
    [ObservableProperty] private bool isCompanyMode;
    [ObservableProperty] private List<TiCollaboratorDto> collaborators = new();

    public TiEventDetailsViewModel(SessionContext session, ITiEventsService eventsService)
    {
        this.session = session;
        this.eventsService = eventsService;
    }

    public async void Load(TiEventDto evt)
    {
        CurrentEvent = evt;
        IsCompanyMode = session.Mode == AppMode.Company;
        await LoadCollaboratorsAsync(evt.Id);
    }

    private async Task LoadCollaboratorsAsync(int eventId)
    {
        var collaborators = await eventsService.GetCollaboratorsAsync(eventId);
        this.collaborators = collaborators;
    }
}
