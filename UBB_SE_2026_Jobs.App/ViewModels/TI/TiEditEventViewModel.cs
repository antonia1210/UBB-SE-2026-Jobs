using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiEditEventViewModel : DispatchableObservableObject
{
    private readonly ITiEventsService eventsService;
    private readonly ICompanyService companyService;
    private readonly SessionContext session;
    private int eventId;

    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string location = string.Empty;
    [ObservableProperty] private DateTimeOffset? startDate;
    [ObservableProperty] private DateTimeOffset? endDate;
    [ObservableProperty] private string titleError = string.Empty;
    [ObservableProperty] private string dateError = string.Empty;
    [ObservableProperty] private bool isSaving;
    [ObservableProperty] private bool updatedSuccessfully;
    [ObservableProperty] private bool deletedSuccessfully;

    public ObservableCollection<TiCompanyPickItem> AvailableCompanies { get; } = new();

    public TiEditEventViewModel(ITiEventsService eventsService, ICompanyService companyService, SessionContext session)
    {
        this.eventsService = eventsService;
        this.companyService = companyService;
        this.session = session;
    }

    public void LoadEvent(TiEventDto eventDto)
    {
        eventId = eventDto.Id;
        Title = eventDto.Title;
        Description = eventDto.Description;
        Location = eventDto.Location;
        StartDate = new DateTimeOffset(eventDto.StartDate);
        EndDate = new DateTimeOffset(eventDto.EndDate);
        _ = LoadCompaniesAsync(eventDto.CollaboratorCompanyIds);
    }

    private async Task LoadCompaniesAsync(List<int> existingCollaboratorIds)
    {
        var companies = await companyService.GetAllAsync();
        foreach (var c in companies)
        {
            if (c.CompanyId != session.CompanyId)
            {
                AvailableCompanies.Add(new TiCompanyPickItem 
                { 
                    Company = new UBB_SE_2026_Jobs.Library.DTOs.CompanyDto { CompanyId = c.CompanyId, Name = c.Name }, 
                    IsSelected = existingCollaboratorIds?.Contains(c.CompanyId) ?? false 
                });
            }
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title)) { TitleError = "Title is required."; return; }
        TitleError = string.Empty;

        IsSaving = true;
        var dto = new TiEventDto
        {
            Id = eventId,
            Title = Title.Trim(),
            Description = Description.Trim(),
            Location = Location.Trim(),
            StartDate = StartDate?.DateTime ?? DateTime.UtcNow,
            EndDate = EndDate?.DateTime ?? DateTime.UtcNow,
            CollaboratorCompanyIds = AvailableCompanies.Where(c => c.IsSelected).Select(c => c.Company.CompanyId).ToList()
        };

        await eventsService.UpdateAsync(eventId, dto);
        IsSaving = false;
        UpdatedSuccessfully = true;
    }

    [RelayCommand]
    public async Task DeleteAsync()
    {
        await eventsService.DeleteAsync(eventId);
        DeletedSuccessfully = true;
    }
}
