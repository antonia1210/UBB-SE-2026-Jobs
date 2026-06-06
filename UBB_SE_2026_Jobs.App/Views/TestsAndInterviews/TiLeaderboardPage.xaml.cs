using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiLeaderboardPage : Page
{
    public TiLeaderboardViewModel ViewModel { get; }

    public TiLeaderboardPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TiLeaderboardViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        int testId = e.Parameter is int id ? id : 0;
        await ViewModel.LoadAsync(testId);
    }

    private void PrevPage_Click(object sender, RoutedEventArgs e) => ViewModel.GoToPrevPage();
    private void NextPage_Click(object sender, RoutedEventArgs e) => ViewModel.GoToNextPage();
}
