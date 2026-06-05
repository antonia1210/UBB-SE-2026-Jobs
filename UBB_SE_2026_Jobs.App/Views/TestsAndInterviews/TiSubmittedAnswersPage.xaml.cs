using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.Dtos.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiSubmittedAnswersPage : Page
{
    public TiSubmittedAnswersViewModel ViewModel { get; }

    public TiSubmittedAnswersPage()
    {
        ViewModel = App.Services.GetRequiredService<TiSubmittedAnswersViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is TiSubmittedAnswersParams p)
            await ViewModel.LoadAsync(p.TestId, p.AttemptId);
    }

    private void Back_Click(object sender, RoutedEventArgs e) => Frame.GoBack();
}
