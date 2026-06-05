using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiManageSlotsViewModel : DispatchableObservableObject
{
    private readonly ITiSlotsService slotsService;
    private readonly SessionContext session;

    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private DateTime weekStart;
    [ObservableProperty] private string weekLabel = string.Empty;
    [ObservableProperty] private TiSlotDto? editingSlot;
    [ObservableProperty] private bool noSlots = true;

    public ObservableCollection<TiSlotDto> Slots { get; } = new();
    public ObservableCollection<TiCompanyDto> Companies { get; } = new();

    public TiManageSlotsViewModel(ITiSlotsService slotsService, SessionContext session)
    {
        this.slotsService = slotsService;
        this.session = session;
        WeekStart = GetMonday(DateTime.Now);
        UpdateWeekLabel();
    }

    partial void OnWeekStartChanged(DateTime value)
    {
        UpdateWeekLabel();
        _ = LoadSlotsAsync();
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            await LoadCompaniesAsync();
            await LoadSlotsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error initializing: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadCompaniesAsync()
    {
        try
        {
            var companies = await slotsService.GetCompaniesAsync();
            await UIDispatcher.EnqueueAsync(() =>
            {
                Companies.Clear();
                foreach (var company in companies)
                    Companies.Add(company);
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Could not load companies: {ex.Message}";
        }
    }

    private async Task LoadSlotsAsync()
    {
        try
        {
            var slots = await slotsService.GetAllSlotsAsync();
            var weekSlots = slots
                .Where(s => s.StartTime >= WeekStart && s.StartTime < WeekStart.AddDays(7))
                .ToList();

            await UIDispatcher.EnqueueAsync(() =>
            {
                Slots.Clear();
                foreach (var slot in weekSlots.OrderBy(s => s.StartTime))
                    Slots.Add(slot);
                NoSlots = Slots.Count == 0;
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Could not load slots: {ex.Message}";
        }
    }

    [RelayCommand]
    public void PreviousWeek()
    {
        WeekStart = WeekStart.AddDays(-7);
    }

    [RelayCommand]
    public void NextWeek()
    {
        WeekStart = WeekStart.AddDays(7);
    }

    [RelayCommand]
    public void Today()
    {
        WeekStart = GetMonday(DateTime.Now);
    }

    public void CreateSlot(int dayIndex, int hour, int minute)
    {
        var date = WeekStart.AddDays(dayIndex).Date;
        var startTime = date.AddHours(hour).AddMinutes(minute);
        EditingSlot = new TiSlotDto
        {
            Id = 0,
            RecruiterId = session.UserId,
            StartTime = startTime,
            EndTime = startTime.AddMinutes(60),
            Duration = 60,
            Status = 0
        };
        if (Companies.Any())
            EditingSlot.CompanyId = Companies.First().CompanyId;
    }

    public void EditSlot(TiSlotDto slot)
    {
        EditingSlot = new TiSlotDto
        {
            Id = slot.Id,
            RecruiterId = slot.RecruiterId,
            CandidateId = slot.CandidateId,
            CompanyId = slot.CompanyId,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Duration = slot.Duration,
            Status = slot.Status,
            InterviewType = slot.InterviewType
        };
    }

    [RelayCommand]
    public async Task SaveSlot()
    {
        if (EditingSlot == null)
            return;

        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            var slot = EditingSlot;

            bool success = slot.Id == 0
                ? await slotsService.CreateSlotAsync(slot)
                : await slotsService.UpdateSlotAsync(slot);

            if (success)
            {
                StatusMessage = slot.Id == 0 ? "Slot created successfully" : "Slot updated successfully";
                EditingSlot = null;
                await LoadSlotsAsync();
            }
            else
            {
                StatusMessage = "Failed to save slot";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving slot: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteSlot()
    {
        if (EditingSlot == null)
            return;

        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            if (await slotsService.DeleteSlotAsync(EditingSlot.Id))
            {
                StatusMessage = "Slot deleted successfully";
                EditingSlot = null;
                await LoadSlotsAsync();
            }
            else
            {
                StatusMessage = "Failed to delete slot";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting slot: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void CancelEdit()
    {
        EditingSlot = null;
        StatusMessage = string.Empty;
    }

    private void UpdateWeekLabel()
    {
        var weekEnd = WeekStart.AddDays(6);
        WeekLabel = $"{WeekStart:d MMM} – {weekEnd:d MMM}";
    }

    private static DateTime GetMonday(DateTime date)
    {
        int diff = date.DayOfWeek - DayOfWeek.Monday;
        if (diff < 0) diff += 7;
        return date.AddDays(-diff).Date;
    }
}