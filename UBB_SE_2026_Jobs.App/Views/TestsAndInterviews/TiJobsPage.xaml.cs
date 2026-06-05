using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services;
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
        await ViewModel.LoadAsync(session.CompanyId);
    }

    private void CreateJob_Click(object sender, RoutedEventArgs e)
        => Frame.Navigate(typeof(TiCreateJobPage));

    private void ViewDetails_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiJobPostingDto job })
            Frame.Navigate(typeof(TiJobDetailsPage), job);
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TiJobPostingDto job })
            Frame.Navigate(typeof(TiCreateJobPage), job);
    }

    private async void DeleteJob_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: TiJobPostingDto job })
            return;

        int applicantCount = await ViewModel.GetApplicantCountAsync(job.JobId);
        bool hasApplicants = applicantCount > 0;

        var dialog = new ContentDialog
        {
            Title = "Delete Job",
            Content = hasApplicants
                ? $"'{job.JobTitle}' has {applicantCount} applicant{(applicantCount == 1 ? "" : "s")}. " +
                  $"Deleting it will permanently remove {(applicantCount == 1 ? "their application" : "their applications")} as well. This cannot be undone."
                : $"Delete '{job.JobTitle}'? This cannot be undone.",
            PrimaryButtonText = hasApplicants ? "Delete anyway" : "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = XamlRoot,
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
            return;

        var result = await ViewModel.DeleteJobAsync(job.JobId, force: hasApplicants);
        if (result != JobDeleteResult.Deleted)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Delete failed",
                Content = "The job could not be deleted. Please try again.",
                CloseButtonText = "OK",
                XamlRoot = XamlRoot,
            };
            await errorDialog.ShowAsync();
        }
    }
}
