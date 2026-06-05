using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiEventsPage : Page
{
    public TiEventsViewModel ViewModel { get; }

    public TiEventsPage()
    {
        ViewModel = App.Services.GetRequiredService<TiEventsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var session = App.Services.GetRequiredService<SessionContext>();
        CreateEventButton.Visibility = session.Mode == AppMode.Company ? Visibility.Visible : Visibility.Collapsed;
        await ViewModel.LoadAsync();
    }

    private void CreateEvent_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(TiCreateEventPage));
    }

    private void ViewEventDetails_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiEventDto dto })
            Frame.Navigate(typeof(TiEventDetailsPage), dto);
    }

    private void EditEvent_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiEventDto dto })
            Frame.Navigate(typeof(TiEditEventPage), dto);
    }

    private async void DeleteEvent_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiEventDto dto })
        {
            var dialog = new ContentDialog
            {
                Title = "Delete Event",
                Content = $"Delete '{dto.Title}'?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot,
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                await ViewModel.DeleteEventAsync(dto.Id);
        }
    }

    public string FormatDate(DateTime value) => value == default ? "—" : value.ToString("dd MMM yyyy");
}
