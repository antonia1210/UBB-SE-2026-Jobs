using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiManageSlotsPage : Page
{
    public TiManageSlotsViewModel ViewModel { get; }

    public TiManageSlotsPage()
    {
        ViewModel = App.Services.GetRequiredService<TiManageSlotsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs eventArguments)
    {
        base.OnNavigatedTo(eventArguments);
        await ViewModel.InitializeAsync();
        RenderCalendar();
        ViewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(ViewModel.CalendarRows))
                RenderCalendar();
        };
    }

    private void PreviousWeek_Click(object sender, RoutedEventArgs eventArguments) => ViewModel.PreviousWeekCommand.Execute(null);
    private void NextWeek_Click(object sender, RoutedEventArgs eventArguments) => ViewModel.NextWeekCommand.Execute(null);
    private void Today_Click(object sender, RoutedEventArgs eventArguments) => ViewModel.TodayCommand.Execute(null);

    private void RenderCalendar()
    {
        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();
        CalendarGrid.ColumnDefinitions.Clear();

        CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
        for (int dayIndex = 0; dayIndex < 7; dayIndex++)
            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        foreach (var _ in ViewModel.CalendarRows)
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        for (int dayIndex = 0; dayIndex < 7; dayIndex++)
        {
            var date = ViewModel.WeekStart.AddDays(dayIndex);
            var isToday = date.Date == DateTime.Now.Date;
            var header = new Border
            {
                Background = isToday
                    ? new SolidColorBrush(Color.FromArgb(255, 221, 214, 254))
                    : new SolidColorBrush(Colors.Transparent),
                Padding = new Thickness(4),
                Child = new TextBlock
                {
                    Text = $"{days[dayIndex]} {date.Day}",
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 13
                }
            };
            Grid.SetColumn(header, dayIndex + 1);
            Grid.SetRow(header, 0);
            CalendarGrid.Children.Add(header);
        }

        int rowIndex = 1;
        foreach (var row in ViewModel.CalendarRows)
        {
            var timeLabel = new TextBlock
            {
                Text = row.TimeLabel,
                FontSize = 11,
                TextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 8, 0),
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };
            Grid.SetColumn(timeLabel, 0);
            Grid.SetRow(timeLabel, rowIndex);
            CalendarGrid.Children.Add(timeLabel);

            for (int columnIndex = 0; columnIndex < row.Cells.Count; columnIndex++)
            {
                var cell = row.Cells[columnIndex];
                FrameworkElement cellElement;

                if (cell.Slot != null)
                {
                    var isBooked = cell.Slot.Status != 0;
                    var slotBackground = isBooked
                        ? Color.FromArgb(255, 165, 214, 167)
                        : Color.FromArgb(255, 221, 214, 254);
                    var slotForeground = isBooked
                        ? Colors.DarkGreen
                        : Color.FromArgb(255, 80, 56, 200);
                    var capturedSlot = cell.Slot;
                    var slotButton = new Button
                    {
                        Content = new TextBlock
                        {
                            Text = $"{capturedSlot.StartTime:HH:mm} · {capturedSlot.Duration}m",
                            FontSize = 11,
                            Foreground = new SolidColorBrush(slotForeground),
                            TextAlignment = TextAlignment.Center
                        },
                        Background = new SolidColorBrush(slotBackground),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        MinHeight = 50,
                        Padding = new Thickness(4, 2, 4, 2),
                        IsEnabled = !isBooked
                    };
                    if (!isBooked)
                        slotButton.Click += async (sender, eventArguments) => await OnSlotClick(capturedSlot);
                    cellElement = slotButton;
                }
                else
                {
                    var capturedCell = cell;
                    var border = new Border
                    {
                        BorderBrush = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                        BorderThickness = new Thickness(0.5),
                        MinHeight = 50,
                        Background = new SolidColorBrush(Colors.Transparent)
                    };
                    border.Tapped += async (sender, eventArguments) => await OnEmptyCellClick(capturedCell.DayIndex, capturedCell.Hour, capturedCell.Minute);
                    cellElement = border;
                }

                Grid.SetColumn(cellElement, columnIndex + 1);
                Grid.SetRow(cellElement, rowIndex);
                CalendarGrid.Children.Add(cellElement);
            }

            rowIndex++;
        }
    }

    private async Task OnEmptyCellClick(int dayIndex, int hour, int minute)
    {
        ViewModel.CreateSlot(dayIndex, hour, minute);
        await ShowSlotDialog();
    }

    private async Task OnSlotClick(TiSlotDto slot)
    {
        ViewModel.EditSlot(slot);
        await ShowSlotDialog();
    }

    private async Task ShowSlotDialog()
    {
        if (ViewModel.EditingSlot == null)
            return;

        var dialog = new ContentDialog
        {
            Title = ViewModel.EditingSlot.Id == 0 ? "Add Slot" : "Edit Slot",
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var stackPanel = new StackPanel { Spacing = 12 };

        var datePicker = new CalendarDatePicker { Date = ViewModel.EditingSlot.StartTime };
        stackPanel.Children.Add(new TextBlock { Text = "Date", FontWeight = FontWeights.SemiBold, FontSize = 12 });
        stackPanel.Children.Add(datePicker);

        var timePicker = new TimePicker { Time = ViewModel.EditingSlot.StartTime.TimeOfDay, ClockIdentifier = "24HourClock" };
        stackPanel.Children.Add(new TextBlock { Text = "Start Time", FontWeight = FontWeights.SemiBold, FontSize = 12 });
        stackPanel.Children.Add(timePicker);

        var durationCombo = new ComboBox();
        durationCombo.Items.Add(new ComboBoxItem { Content = "60 min", Tag = "60" });
        durationCombo.Items.Add(new ComboBoxItem { Content = "90 min", Tag = "90" });
        durationCombo.SelectedIndex = ViewModel.EditingSlot.Duration == 90 ? 1 : 0;
        stackPanel.Children.Add(new TextBlock { Text = "Duration (minutes)", FontWeight = FontWeights.SemiBold, FontSize = 12 });
        stackPanel.Children.Add(durationCombo);

        var companyCombo = new ComboBox { ItemsSource = ViewModel.Companies.ToList() };
        companyCombo.DisplayMemberPath = "Name";
        companyCombo.SelectedItem = ViewModel.Companies.FirstOrDefault(company => company.CompanyId == ViewModel.EditingSlot.CompanyId)
                                    ?? ViewModel.Companies.FirstOrDefault();
        stackPanel.Children.Add(new TextBlock { Text = "Company", FontWeight = FontWeights.SemiBold, FontSize = 12 });
        stackPanel.Children.Add(companyCombo);

        if (ViewModel.EditingSlot.Id != 0)
        {
            var capturedSlot = ViewModel.EditingSlot;
            var deleteBtn = new Button
            {
                Content = "Delete Slot",
                Background = new SolidColorBrush(Color.FromArgb(255, 247, 99, 12)),
                Foreground = new SolidColorBrush(Colors.White)
            };
            deleteBtn.Click += async (sender, eventArguments) =>
            {
                dialog.Hide();
                await DeleteSlot_Click(capturedSlot);
            };
            stackPanel.Children.Add(deleteBtn);
        }

        dialog.Content = stackPanel;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var date = datePicker.Date ?? DateTimeOffset.Now;
            var time = timePicker.Time;
            var startTime = date.DateTime.Date.Add(time);
            var duration = int.Parse(((ComboBoxItem)durationCombo.SelectedItem)?.Tag?.ToString() ?? "60");
            var companyId = (companyCombo.SelectedItem as TiCompanyDto)?.CompanyId;

            if (companyId.HasValue)
            {
                ViewModel.EditingSlot.StartTime = startTime;
                ViewModel.EditingSlot.EndTime = startTime.AddMinutes(duration);
                ViewModel.EditingSlot.Duration = duration;
                ViewModel.EditingSlot.CompanyId = companyId.Value;
                await ViewModel.SaveSlotCommand.ExecuteAsync(null);
            }
            else
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please select a company",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
        else
        {
            ViewModel.CancelEditCommand.Execute(null);
        }
    }

    private async Task DeleteSlot_Click(TiSlotDto slot)
    {
        var confirmDialog = new ContentDialog
        {
            Title = "Delete Slot",
            Content = $"Delete this slot on {slot.DisplayDate}?",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            XamlRoot = this.XamlRoot
        };

        if (await confirmDialog.ShowAsync() == ContentDialogResult.Primary)
            await ViewModel.DeleteSlotDirectAsync(slot.Id);
    }
}
