using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.Jobs;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiJobsViewModel : DispatchableObservableObject
{
    private readonly IJobService jobService;
    private readonly SessionContext session;

    [ObservableProperty] private bool isLoading;

    public ObservableCollection<TiJobPostingDto> Jobs { get; } = new();

    public TiJobsViewModel(IJobService jobService, SessionContext session)
    {
        this.jobService = jobService;
        this.session = session;
    }

    public bool IsCompanyMode => session.Mode == AppMode.Company;

    public async Task LoadAsync(int? companyId = null)
    {
        IsLoading = true;
        var jobs = companyId.HasValue
            ? await jobService.GetByCompanyIdAsync(companyId.Value)
            : await jobService.GetAllAsync();
        Jobs.Clear();
        foreach (var job in jobs)
        {
            var dto = TiJobMapper.ToDto(job);
            dto.IsCompanyMode = IsCompanyMode;
            Jobs.Add(dto);
        }
        IsLoading = false;
    }

    public async Task<int> GetApplicantCountAsync(int jobId)
        => await jobService.GetApplicantCountAsync(jobId);

    public async Task<JobDeleteResult> DeleteJobAsync(int jobId, bool force)
    {
        var result = await jobService.RemoveAsync(jobId, force);
        if (result == JobDeleteResult.Deleted)
            await LoadAsync(session.CompanyId);
        return result;
    }
}
