using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.Services.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public partial class TiInterviewSlotsViewModel : DispatchableObservableObject
{
    private readonly ITiSlotsService slotsService;
    private readonly SessionContext session;

    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private DateTimeOffset selectedDate = DateTimeOffset.Now;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string selectedDateLabel = string.Empty;
    [ObservableProperty] private bool noBookings = true;
    [ObservableProperty] private bool noAvailableSlots = true;
    [ObservableProperty] private TiApplicationDto? selectedApplication;
    [ObservableProperty] private bool noApplications = true;

    public ObservableCollection<TiApplicationDto> Applications { get; } = new();
    public ObservableCollection<TiSlotDto> AvailableSlots { get; } = new();
    public ObservableCollection<TiSlotDto> MyBookings { get; } = new();
    public HashSet<DateTime> BookedDates { get; } = new();

    public TiInterviewSlotsViewModel(ITiSlotsService slotsService, SessionContext session)
    {
        this.slotsService = slotsService;
        this.session = session;
    }

    partial void OnSelectedDateChanged(DateTimeOffset value)
    {
        SelectedDateLabel = value.ToString("dddd, dd MMM yyyy");
        _ = LoadAvailableSlotsAsync();
    }

    partial void OnSelectedApplicationChanged(TiApplicationDto? value)
    {
        _ = LoadAvailableSlotsAsync();
    }

    public async Task LoadSlotsAsync()
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        SelectedDateLabel = SelectedDate.ToString("dddd, dd MMM yyyy");
        try
        {
            var applicationsTask = slotsService.GetApplicationsAsync(session.UserId);
            var bookingsTask = slotsService.GetMyBookingsAsync(session.UserId);
            await System.Threading.Tasks.Task.WhenAll(applicationsTask, bookingsTask);

            var applications = applicationsTask.Result;
            var bookings = bookingsTask.Result;

            await UIDispatcher.EnqueueAsync(() =>
            {
                Applications.Clear();
                foreach (var app in applications)
                    Applications.Add(app);
                NoApplications = Applications.Count == 0;

                MyBookings.Clear();
                BookedDates.Clear();
                foreach (var s in bookings)
                {
                    MyBookings.Add(s);
                    BookedDates.Add(s.StartTime.Date);
                }
                NoBookings = MyBookings.Count == 0;

                // Auto-select first application if available
                if (Applications.Count > 0 && SelectedApplication == null)
                    SelectedApplication = Applications[0];
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Could not load applications: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAvailableSlotsAsync()
    {
        if (SelectedApplication == null)
        {
            AvailableSlots.Clear();
            NoAvailableSlots = true;
            return;
        }

        try
        {
            var companyId = await slotsService.GetCompanyIdByJobAsync(SelectedApplication.JobId);
            var slots = await slotsService.GetAvailableAsync(SelectedDate.Date, companyId);
            await UIDispatcher.EnqueueAsync(() =>
            {
                AvailableSlots.Clear();
                foreach (var s in slots.Where(s => s.Status == 0))
                    AvailableSlots.Add(s);
                NoAvailableSlots = AvailableSlots.Count == 0;
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Could not load slots: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task BookSlotAsync(TiSlotDto slot)
    {
        if (SelectedApplication == null)
        {
            StatusMessage = "Please select an application first.";
            return;
        }

        try
        {
            bool success = await slotsService.BookSlotAsync(slot.Id, session.UserId, SelectedApplication.JobId);
            StatusMessage = success ? "Slot booked successfully!" : "Failed to book slot. Please try again.";
            if (success) await LoadSlotsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Booking failed: {ex.Message}";
        }
    }
}
