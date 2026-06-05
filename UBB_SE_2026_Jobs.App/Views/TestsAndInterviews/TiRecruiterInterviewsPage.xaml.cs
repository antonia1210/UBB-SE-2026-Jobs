using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiRecruiterInterviewsPage : Page
{
    public TiRecruiterInterviewsViewModel ViewModel { get; }

    public TiRecruiterInterviewsPage()
    {
        ViewModel = App.Services.GetRequiredService<TiRecruiterInterviewsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAllAsync();
    }

    private void Calendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs e)
    {
        if (e.AddedDates.Count > 0)
            ViewModel.SelectedDate = new DateTimeOffset(e.AddedDates[0].DateTime);
    }

    private void ManageSlots_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(TiManageSlotsPage));
    }
}
