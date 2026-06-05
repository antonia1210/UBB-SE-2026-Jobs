using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.Skills;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiCreateJobViewModel : DispatchableObservableObject
{
    private readonly IJobService jobService;
    private readonly ISkillService skillService;
    private readonly SessionContext session;

    [ObservableProperty] private string jobTitle = string.Empty;
    [ObservableProperty] private string industryField = string.Empty;
    [ObservableProperty] private string jobType = string.Empty;
    [ObservableProperty] private string experienceLevel = string.Empty;
    [ObservableProperty] private string jobDescription = string.Empty;
    [ObservableProperty] private string jobLocation = string.Empty;
    [ObservableProperty] private string salaryText = string.Empty;
    [ObservableProperty] private DateTimeOffset? startDate;
    [ObservableProperty] private DateTimeOffset? endDate;
    [ObservableProperty] private DateTimeOffset? deadline;
    [ObservableProperty] private double availablePositions = 1;
    [ObservableProperty] private bool isSaving;
    [ObservableProperty] private bool savedSuccessfully;
    [ObservableProperty] private string validationError = string.Empty;
    [ObservableProperty] private bool isEditMode;

    // Fields preserved across an edit so the update does not clobber columns the form does not show.
    private int editingJobId;
    private int editingCompanyId;
    private DateTime? editingPostedAt;
    private string? editingPhoto;
    private int? editingAmountPayed;

    public ObservableCollection<TiSkillPickItem> SkillRows { get; } = new();

    public TiCreateJobViewModel(IJobService jobService, ISkillService skillService, SessionContext session)
    {
        this.jobService = jobService;
        this.skillService = skillService;
        this.session = session;
    }

    public async Task InitializeAsync()
    {
        var skills = await skillService.GetAllAsync();
        SkillRows.Clear();
        foreach (var skill in skills)
            SkillRows.Add(new TiSkillPickItem { Skill = TiJobMapper.ToDto(skill) });
    }

    /// <summary>
    /// Switches the form into edit mode and prefills it from an existing job. Required-skill edits
    /// are not exposed here (mirroring the web edit form); the update preserves existing skill links.
    /// </summary>
    public void LoadForEdit(TiJobPostingDto job)
    {
        IsEditMode = true;
        editingJobId = job.JobId;
        editingCompanyId = job.CompanyId;
        editingPostedAt = job.PostedAt;
        editingPhoto = job.Photo;
        editingAmountPayed = job.AmountPayed;

        JobTitle = job.JobTitle ?? string.Empty;
        IndustryField = job.IndustryField ?? string.Empty;
        JobType = job.JobType ?? string.Empty;
        ExperienceLevel = job.ExperienceLevel ?? string.Empty;
        JobDescription = job.JobDescription ?? string.Empty;
        JobLocation = job.JobLocation ?? string.Empty;
        SalaryText = job.Salary?.ToString() ?? string.Empty;
        AvailablePositions = job.AvailablePositions < 1 ? 1 : job.AvailablePositions;
        StartDate = job.StartDate.HasValue ? new DateTimeOffset(job.StartDate.Value) : null;
        EndDate = job.EndDate.HasValue ? new DateTimeOffset(job.EndDate.Value) : null;
        Deadline = job.Deadline.HasValue ? new DateTimeOffset(job.Deadline.Value) : null;
    }

    [RelayCommand]
    public async Task SaveJobAsync()
    {
        ValidationError = string.Empty;

        if (string.IsNullOrWhiteSpace(JobTitle)) { ValidationError = "Job title is required."; return; }
        if (string.IsNullOrWhiteSpace(JobType)) { ValidationError = "Job type is required."; return; }
        if (AvailablePositions < 1) { ValidationError = "Available positions must be at least 1."; return; }

        int? salary = null;
        if (!string.IsNullOrWhiteSpace(SalaryText))
        {
            if (!int.TryParse(SalaryText, out int salaryNumber) || salaryNumber < 0) { ValidationError = "Salary must be a positive number."; return; }
            salary = salaryNumber;
        }

        IsSaving = true;

        var job = new Job
        {
            JobTitle = JobTitle.Trim(),
            IndustryField = IndustryField.Trim(),
            JobType = JobType,
            ExperienceLevel = ExperienceLevel,
            JobDescription = JobDescription.Trim(),
            JobLocation = JobLocation.Trim(),
            AvailablePositions = (int)AvailablePositions,
            Salary = salary,
            StartDate = StartDate?.DateTime,
            EndDate = EndDate?.DateTime,
            Deadline = Deadline?.DateTime,
            // Preserve the original company/posted/photo/payment when editing; default for new jobs.
            CompanyId = IsEditMode ? editingCompanyId : (session.CompanyId ?? 1),
            PostedAt = IsEditMode ? editingPostedAt : DateTime.UtcNow,
            Photo = IsEditMode ? editingPhoto : null,
            AmountPayed = IsEditMode ? (editingAmountPayed ?? 0) : 0,
        };

        if (IsEditMode)
        {
            job.JobId = editingJobId;
            await jobService.UpdateAsync(job);
        }
        else
        {
            await jobService.AddAsync(job);
        }

        IsSaving = false;
        SavedSuccessfully = true;
    }
}
