using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiAlreadySubmittedPage : Page
{
    public TiAlreadySubmittedViewModel ViewModel { get; }

    private int loadedTestId;

    public TiAlreadySubmittedPage()
    {
        ViewModel = App.Services.GetRequiredService<TiAlreadySubmittedViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is int testId)
        {
            loadedTestId = testId;
            await ViewModel.LoadAsync(testId);
        }
    }

    private void ReturnToDashboard_Click(object sender, RoutedEventArgs e) =>
        Frame.Navigate(typeof(TiMainTestPage));

    private void ViewAnswers_Click(object sender, RoutedEventArgs e) =>
        Frame.Navigate(typeof(TiSubmittedAnswersPage), new TiSubmittedAnswersParams(loadedTestId, ViewModel.AttemptId));
}
