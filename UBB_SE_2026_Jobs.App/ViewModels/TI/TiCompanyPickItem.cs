using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiCompanyPickItem : ObservableObject
{
    [ObservableProperty]
    private CompanyDto company = null!;

    [ObservableProperty]
    private bool isSelected;
}
