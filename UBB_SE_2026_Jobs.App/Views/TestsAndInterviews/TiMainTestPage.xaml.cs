using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiMainTestPage : Page
{
    public TiMainTestViewModel ViewModel { get; }

    public TiMainTestPage()
    {
        ViewModel = App.Services.GetRequiredService<TiMainTestViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadTestsAsync();
    }

    private void ViewDetails_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiTestCardViewModel card })
        {
            ViewModel.SelectedTest = card;
            Frame.Navigate(typeof(TiTestDetailsPage), card);
        }
    }

    private void ViewLeaderboard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiTestCardViewModel card })
            Frame.Navigate(typeof(TiLeaderboardPage), card.TestId);
    }

    private void StartTest_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiTestCardViewModel card })
        {
            ViewModel.SelectedTest = card;
            Frame.Navigate(typeof(TiTestPage), card.TestId);
        }
    }

    private void Card_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: TiTestCardViewModel card })
            card.IsHovered = true;
    }

    private void Card_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: TiTestCardViewModel card })
            card.IsHovered = false;
    }
}
