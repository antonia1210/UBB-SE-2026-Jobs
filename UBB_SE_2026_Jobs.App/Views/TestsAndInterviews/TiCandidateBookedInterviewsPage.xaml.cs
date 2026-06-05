using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiCandidateBookedInterviewsPage : Page
{
    public TiCandidateBookedInterviewsViewModel ViewModel { get; }

    public TiCandidateBookedInterviewsPage()
    {
        ViewModel = App.Services.GetRequiredService<TiCandidateBookedInterviewsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadBookedInterviewsAsync();
    }
}
