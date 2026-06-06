using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.Library.ServiceProxies.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiApplicantsViewModel : DispatchableObservableObject
{
    private readonly ITiApplicantService applicantService;

    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private TiJobPostingDto? currentJob;

    public ObservableCollection<TiMatchSummaryDto> Applicants { get; } = new();

    public TiApplicantsViewModel(ITiApplicantService applicantService)
    {
        this.applicantService = applicantService;
    }

    public async Task LoadForJobAsync(TiJobPostingDto job)
    {
        CurrentJob = job;
        IsLoading = true;
        var list = await applicantService.GetByJobAsync(job.JobId);
        Applicants.Clear();
        foreach (var applicant in list) Applicants.Add(applicant);
        IsLoading = false;
    }
}
