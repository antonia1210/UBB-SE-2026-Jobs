using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiTestPage : Page
{
    private bool isActive;

    public TiTestPageViewModel ViewModel { get; }

    public TiTestPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TiTestPageViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        isActive = true;

        int testId = e.Parameter is int id ? id : 0;
        var session = App.Services.GetRequiredService<SessionContext>();
        int userId = session.UserId;

        ViewModel.OnTimerExpired = async () =>
        {
            if (!isActive || App.IsShuttingDown)
            {
                return;
            }

            await ViewModel.SubmitAsync();
            await ShowResultDialogAsync("Time's up!", "Your test has been auto-submitted.");
            if (isActive && !App.IsShuttingDown)
            {
                Frame.Navigate(typeof(TiMainTestPage));
            }
        };

        await ViewModel.LoadAsync(testId, userId);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        Shutdown();
        base.OnNavigatedFrom(e);
    }

    private async void SubmitTest_Click(object sender, RoutedEventArgs e)
    {
        if (!isActive || App.IsShuttingDown)
        {
            return;
        }

        float score = await ViewModel.SubmitAsync();
        await ShowResultDialogAsync("Test Submitted!", $"Your score: {score:0.##}");
        if (isActive && !App.IsShuttingDown)
        {
            Frame.Navigate(typeof(TiMainTestPage));
        }
    }

    public void Shutdown()
    {
        isActive = false;
        ViewModel.StopTimer();
        ViewModel.OnTimerExpired = null;
    }

    private async Task ShowResultDialogAsync(string title, string message)
    {
        if (!isActive || App.IsShuttingDown || XamlRoot is null)
        {
            return;
        }

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }
}
