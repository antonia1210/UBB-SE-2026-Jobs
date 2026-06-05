using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;
using UBB_SE_2026_Jobs.Library.Services.Jobs;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiJobsViewModel : DispatchableObservableObject
{
    private readonly IJobService jobService;

    [ObservableProperty] private bool isLoading;

    public ObservableCollection<TiJobPostingDto> Jobs { get; } = new();

    public TiJobsViewModel(IJobService jobService)
    {
        this.jobService = jobService;
    }

    public async Task LoadAsync(int? companyId = null)
    {
        IsLoading = true;
        var jobs = companyId.HasValue
            ? await jobService.GetByCompanyIdAsync(companyId.Value)
            : await jobService.GetAllAsync();
        Jobs.Clear();
        foreach (var job in jobs) Jobs.Add(TiJobMapper.ToDto(job));
        IsLoading = false;
    }

    public async Task DeleteJobAsync(int jobId)
    {
        await jobService.RemoveAsync(jobId);
        await LoadAsync();
    }
}
