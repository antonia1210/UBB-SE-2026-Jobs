using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.Library.ServiceProxies.TI;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiCreateEventViewModel : DispatchableObservableObject
{
    private readonly ITiEventsService eventsService;
    private readonly ICompanyService companyService;
    private readonly SessionContext session;

    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string location = string.Empty;
    [ObservableProperty] private DateTimeOffset? startDate;
    [ObservableProperty] private DateTimeOffset? endDate;
    [ObservableProperty] private string titleError = string.Empty;
    [ObservableProperty] private string descriptionError = string.Empty;
    [ObservableProperty] private string locationError = string.Empty;
    [ObservableProperty] private string dateError = string.Empty;
    [ObservableProperty] private bool isSaving;
    [ObservableProperty] private bool createdSuccessfully;

    public ObservableCollection<TiCompanyPickItem> AvailableCompanies { get; } = new();

    public TiCreateEventViewModel(ITiEventsService eventsService, ICompanyService companyService, SessionContext session)
    {
        this.eventsService = eventsService;
        this.companyService = companyService;
        this.session = session;
        _ = LoadCompaniesAsync();
    }

    private async Task LoadCompaniesAsync()
    {
        var companies = await companyService.GetAllAsync();
        foreach (var c in companies)
        {
            // Do not include the host company as a collaborator
            if (c.CompanyId != session.CompanyId)
            {
                AvailableCompanies.Add(new TiCompanyPickItem 
                { 
                    Company = new UBB_SE_2026_Jobs.Library.DTOs.CompanyDto { CompanyId = c.CompanyId, Name = c.Name }, 
                    IsSelected = false 
                });
            }
        }
    }

    [RelayCommand]
    public async Task CreateEventAsync()
    {
        if (!Validate()) return;

        IsSaving = true;
        var dto = new TiEventDto
        {
            Title = Title.Trim(),
            Description = Description.Trim(),
            Location = Location.Trim(),
            StartDate = StartDate!.Value.DateTime,
            EndDate = EndDate!.Value.DateTime,
            HostCompanyId = session.CompanyId ?? 1,
            PostedAt = DateTime.UtcNow,
            CollaboratorCompanyIds = AvailableCompanies.Where(c => c.IsSelected).Select(c => c.Company.CompanyId).ToList()
        };

        await eventsService.CreateAsync(dto);
        IsSaving = false;
        CreatedSuccessfully = true;
    }

    private bool Validate()
    {
        bool ok = true;
        TitleError = string.Empty;
        DescriptionError = string.Empty;
        LocationError = string.Empty;
        DateError = string.Empty;

        if (string.IsNullOrWhiteSpace(Title)) { TitleError = "Title is required."; ok = false; }
        if (string.IsNullOrWhiteSpace(Description)) { DescriptionError = "Description is required."; ok = false; }
        if (string.IsNullOrWhiteSpace(Location)) { LocationError = "Location is required."; ok = false; }
        if (StartDate == null) { DateError = "Start date is required."; ok = false; }
        if (EndDate == null) { DateError = "End date is required."; ok = false; }
        if (StartDate != null && EndDate != null && EndDate < StartDate)
        { DateError = "End date must be after start date."; ok = false; }
        return ok;
    }
}
