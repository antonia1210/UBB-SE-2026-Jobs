using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Dtos.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiSkillPickItem : ObservableObject
{
    [ObservableProperty] private bool isSelected;
    [ObservableProperty] private string requiredPercentText = "50";
    public TiSkillDto Skill { get; init; } = new();
}
