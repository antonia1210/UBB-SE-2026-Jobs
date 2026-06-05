using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiCreateEventPage : Page
{
    public TiCreateEventViewModel ViewModel { get; }

    public TiCreateEventPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TiCreateEventViewModel>();
    }

    private async void Create_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.CreateEventAsync();
        if (ViewModel.CreatedSuccessfully)
            Frame.Navigate(typeof(TiEventsPage));
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Frame.GoBack();
}
