using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;
using System.Diagnostics;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiInterviewSlotsPage : Page
{
    public TiInterviewSlotsViewModel ViewModel { get; }

    public TiInterviewSlotsPage()
    {
        ViewModel = App.Services.GetRequiredService<TiInterviewSlotsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadSlotsAsync();
    }

    private void ApplicationCard_Click(object sender, TappedRoutedEventArgs e)
    {
        if (sender is Border border && border.Tag is TiApplicationDto app)
        {
            ViewModel.SelectedApplication = app;
            Debug.WriteLine(app.JobId);
        }
    }

    private void AppCard_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border)
        {
            border.BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AccentFillColorDefaultBrush"];
        }
    }

    private void AppCard_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border)
        {
            border.BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"];
        }
    }

    private void DatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
    {
        if (args.NewDate.HasValue)
        {
            ViewModel.SelectedDate = args.NewDate.Value;
        }
    }

    private async void BookSlot_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiSlotDto slot })
            await ViewModel.BookSlotAsync(slot);
    }

    private void ViewBookedInterviews_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(TiCandidateBookedInterviewsPage));
    }
}
