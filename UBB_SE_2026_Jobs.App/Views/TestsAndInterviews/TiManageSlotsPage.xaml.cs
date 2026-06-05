using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Text;
using Windows.UI;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiManageSlotsPage : Page
{
    private const int START_HOUR = 8;
    private const int END_HOUR = 18;

    public TiManageSlotsViewModel ViewModel { get; }

    public TiManageSlotsPage()
    {
        ViewModel = App.Services.GetRequiredService<TiManageSlotsViewModel>();
        InitializeComponent();
        this.DataContext = this;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.InitializeAsync();
        RenderCalendar();
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        UpdateWeekLabel();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Slots) || e.PropertyName == nameof(ViewModel.WeekStart))
        {
            RenderCalendar();
            UpdateWeekLabel();
        }
    }

    private void UpdateWeekLabel()
    {
        WeekLabel.Text = ViewModel.WeekLabel;
        NoSlotsMessage.Visibility = ViewModel.NoSlots ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }

    private void PreviousWeek_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.PreviousWeekCommand.Execute(null);
    }

    private void NextWeek_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.NextWeekCommand.Execute(null);
    }

    private void Today_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.TodayCommand.Execute(null);
    }

    private void RenderCalendar()
    {
        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();
        CalendarGrid.ColumnDefinitions.Clear();

        CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int h = START_HOUR; h < END_HOUR; h++)
        {
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        for (int i = 0; i < 7; i++)
        {
            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        for (int i = 0; i < 7; i++)
        {
            var date = ViewModel.WeekStart.AddDays(i);
            var isToday = date.Date == DateTime.Now.Date;
            var header = new TextBlock
            {
                Text = $"{days[i]}\n{date.Day}",
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.SemiBold,
                Foreground = isToday ? new SolidColorBrush(Colors.Blue) : null
            };
            Grid.SetColumn(header, i + 1);
            Grid.SetRow(header, 0);
            CalendarGrid.Children.Add(header);
        }

        int rowIndex = 1;
        for (int h = START_HOUR; h < END_HOUR; h++)
        {
            for (int m = 0; m < 60; m += 30)
            {
                var timeLabel = new TextBlock
                {
                    Text = $"{h:D2}:{m:D2}",
                    TextAlignment = TextAlignment.Right,
                    Padding = new Thickness(8, 4, 8, 4),
                    FontSize = 11,
                    Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
                };
                Grid.SetColumn(timeLabel, 0);
                Grid.SetRow(timeLabel, rowIndex);
                CalendarGrid.Children.Add(timeLabel);

                for (int day = 0; day < 7; day++)
                {
                    var cellDate = ViewModel.WeekStart.AddDays(day).Date;
                    var cellSlots = ViewModel.Slots
                        .Where(s => s.StartTime.Date == cellDate &&
                                    s.StartTime.Hour == h &&
                                    s.StartTime.Minute == m)
                        .ToList();

                    if (cellSlots.Count > 0)
                    {
                        foreach (var slot in cellSlots)
                        {
                            var slotButton = CreateSlotButton(slot);
                            Grid.SetColumn(slotButton, day + 1);
                            Grid.SetRow(slotButton, rowIndex);
                            CalendarGrid.Children.Add(slotButton);
                        }
                    }
                    else
                    {
                        int capturedDay = day;
                        var emptyCell = new Button
                        {
                            Content = string.Empty,
                            Padding = new Thickness(0),
                            MinHeight = 50,
                            Background = new SolidColorBrush(Colors.Transparent),
                            BorderBrush = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"]
                        };
                        emptyCell.Click += (s, e) => CellClick(capturedDay, h, m);
                        Grid.SetColumn(emptyCell, day + 1);
                        Grid.SetRow(emptyCell, rowIndex);
                        CalendarGrid.Children.Add(emptyCell);
                    }
                }

                rowIndex++;
            }
        }
    }

    private Button CreateSlotButton(TiSlotDto slot)
    {
        var isBooked = slot.Status != 0;
        var bgColor = isBooked
            ? Color.FromArgb(255, 165, 214, 167)
            : Color.FromArgb(255, 221, 214, 254);
        var fgColor = isBooked
            ? Colors.DarkGreen
            : Color.FromArgb(255, 80, 56, 200);

        var button = new Button
        {
            Content = new TextBlock
            {
                Text = $"{slot.StartTime:HH:mm} · {slot.Duration}m",
                Foreground = new SolidColorBrush(fgColor),
                FontSize = 11,
                TextAlignment = TextAlignment.Center
            },
            Background = new SolidColorBrush(bgColor),
            Padding = new Thickness(4, 2, 4, 2),
            MinHeight = 50,
            IsEnabled = !isBooked
        };

        if (!isBooked)
            button.Click += (s, e) => SlotClick(slot);

        return button;
    }

    private async void CellClick(int dayIndex, int hour, int minute)
    {
        ViewModel.CreateSlot(dayIndex, hour, minute);
        await ShowSlotDialog();
    }

    private async void SlotClick(TiSlotDto slot)
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

        var companyCombo = new ComboBox
        {
            ItemsSource = ViewModel.Companies.ToList()
        };
        companyCombo.DisplayMemberPath = "Name";
        var matchingCompany = ViewModel.Companies.FirstOrDefault(c => c.CompanyId == ViewModel.EditingSlot.CompanyId);
        companyCombo.SelectedItem = matchingCompany ?? ViewModel.Companies.FirstOrDefault();
        stackPanel.Children.Add(new TextBlock { Text = "Company", FontWeight = FontWeights.SemiBold, FontSize = 12 });
        stackPanel.Children.Add(companyCombo);

        if (ViewModel.EditingSlot.Id != 0)
        {
            var deleteBtn = new Button
            {
                Content = "Delete Slot",
                Background = new SolidColorBrush(Color.FromArgb(255, 247, 99, 12)),
                Foreground = new SolidColorBrush(Colors.White)
            };
            deleteBtn.Click += async (s, e) =>
            {
                dialog.Hide();
                await DeleteSlot_Click();
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

    private async Task DeleteSlot_Click()
    {
        if (ViewModel.EditingSlot == null)
            return;

        var confirmDialog = new ContentDialog
        {
            Title = "Delete Slot",
            Content = $"Delete this slot on {ViewModel.EditingSlot.DisplayDate}?",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            XamlRoot = this.XamlRoot,
        };

        var result = await confirmDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.DeleteSlotCommand.ExecuteAsync(null);
        }
    }
}