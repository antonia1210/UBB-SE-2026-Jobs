using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.ViewModels;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.App.Views.Controls;
using UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

namespace UBB_SE_2026_Jobs.App.Views.Candidate;

public sealed partial class TestDashboardPage : Page
{
    private readonly TestDashboardViewModel viewModel;

    public TestDashboardPage()
    {
        InitializeComponent();
        viewModel = App.Services.GetRequiredService<TestDashboardViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs eventArguments)
    {
        base.OnNavigatedTo(eventArguments);
        var user = eventArguments.Parameter as User;
        await viewModel.LoadTestsAsync(user);
        RenderCards();
    }

    private void RenderCards()
    {
        TestCardsContainer.Children.Clear();
        TestCountText.Text = viewModel.TestCards.Count.ToString();

        if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
        {
            TestCardsContainer.Children.Add(new TextBlock
            {
                Text = viewModel.ErrorMessage,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                FontSize = 14,
            });
            return;
        }

        foreach (var cardViewModel in viewModel.TestCards)
        {
            var card = new SkillTestCardControl(cardViewModel);
            card.TakeTestRequested += OnTakeTestRequested;
            TestCardsContainer.Children.Add(card);
        }
    }

    private void OnTakeTestRequested(object? sender, int testId)
    {
        Frame.Navigate(typeof(TiTestPage), testId);
    }
}