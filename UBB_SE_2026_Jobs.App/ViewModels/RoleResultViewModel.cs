using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.App.ViewModels;

public class RoleResultViewModel : DispatchableObservableObject
{
    private bool isSelected;

    public RoleResultViewModel(JobRole jobRole, double score)
    {
        Role = jobRole;
        Score = score;
    }

    public JobRole Role { get; }
    public double Score { get; }
    public string DisplayName => ViewModelSupport.FormatJobRole(Role);

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }
}
