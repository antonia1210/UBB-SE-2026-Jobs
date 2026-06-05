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

public sealed partial class TiJobsPage : Page
{
    public TiJobsViewModel ViewModel { get; }

    public TiJobsPage()
    {
        ViewModel = App.Services.GetRequiredService<TiJobsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var session = App.Services.GetRequiredService<SessionContext>();
        PostJobButton.Visibility = session.Mode == AppMode.Company ? Visibility.Visible : Visibility.Collapsed;
        await ViewModel.LoadAsync();
    }

    private void CreateJob_Click(object sender, RoutedEventArgs e)
        => Frame.Navigate(typeof(TiCreateJobPage));

    private void ViewDetails_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiJobPostingDto job })
            Frame.Navigate(typeof(TiJobDetailsPage), job);
    }

    private void ViewApplicants_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiJobPostingDto job })
            Frame.Navigate(typeof(TiJobApplicantsPage), job);
    }

    private void Pay_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiJobPostingDto job })
            Frame.Navigate(typeof(TiPaymentPage), job);
    }

    private async void DeleteJob_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiJobPostingDto job })
        {
            var dialog = new ContentDialog
            {
                Title = "Delete Job",
                Content = $"Delete '{job.JobTitle}'?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot,
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                await ViewModel.DeleteJobAsync(job.JobId);
        }
    }
}
