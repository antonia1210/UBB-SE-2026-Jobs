using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiEventDetailsViewModel : DispatchableObservableObject
{
    private readonly SessionContext session;

    [ObservableProperty] private TiEventDto? currentEvent;
    [ObservableProperty] private bool isCompanyMode;

    public TiEventDetailsViewModel(SessionContext session)
    {
        this.session = session;
    }

    public void Load(TiEventDto evt)
    {
        CurrentEvent = evt;
        IsCompanyMode = session.Mode == AppMode.Company;
    }
}
