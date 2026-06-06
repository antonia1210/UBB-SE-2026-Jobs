using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;

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

    private void ManageSlots_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(TiManageSlotsPage));
    }

    private async void DecideButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not TiInterviewSessionDto session)
            return;

        var dialog = new ContentDialog
        {
            Title = "Candidate Decision",
            Content = $"Session #{session.Id}\nCandidate #{session.ExternalUserId}\n\nChoose result for candidate:",
            PrimaryButtonText = "Accept",
            SecondaryButtonText = "Decline",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
            await ViewModel.SubmitDecisionAsync(session.Id, "Accepted");
        else if (result == ContentDialogResult.Secondary)
            await ViewModel.SubmitDecisionAsync(session.Id, "Declined");
    }
}