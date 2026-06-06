using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiJobApplicantsPage : Page
{
    public TiApplicantsViewModel ViewModel { get; }

    public TiJobApplicantsPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TiApplicantsViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is TiJobPostingDto job)
            await ViewModel.LoadForJobAsync(job);
    }

}
