using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiEditEventPage : Page
{
    public TiEditEventViewModel ViewModel { get; }

    public TiEditEventPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TiEditEventViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is TiEventDto dto)
            ViewModel.LoadEvent(dto);
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.SaveAsync();
        if (ViewModel.UpdatedSuccessfully)
            Frame.Navigate(typeof(TiEventsPage));
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.DeleteAsync();
        if (ViewModel.DeletedSuccessfully)
            Frame.Navigate(typeof(TiEventsPage));
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Frame.GoBack();
}
