using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using UBB_SE_2026_Jobs.App.ViewModels;

namespace UBB_SE_2026_Jobs.App.Views.Controls;

public sealed partial class SkillTestCardControl : UserControl
{
    private readonly SkillTestCardViewModel viewModel;

    public event EventHandler<int>? TakeTestRequested;

    public SkillTestCardControl(SkillTestCardViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        LoadCard();
    }

    private void LoadCard()
    {
        TestNameText.Text = viewModel.Title.ToUpper();
        ScoreText.Text = viewModel.ScoreText;
        DateText.Text = string.IsNullOrEmpty(viewModel.DateText)
            ? viewModel.Status
            : $"{viewModel.Status} · {viewModel.DateText}";

        string tierLabel = viewModel.Badge?.Tier.ToString() ?? "Participant";
        TierLabel.Text = tierLabel;

        // Background tint and text color for tier badge on light card
        TierBackground.Color = tierLabel switch
        {
            "Gold" => Windows.UI.Color.FromArgb(255, 255, 236, 153),
            "Silver" => Windows.UI.Color.FromArgb(255, 220, 220, 230),
            "Bronze" => Windows.UI.Color.FromArgb(255, 235, 195, 155),
            _ => Windows.UI.Color.FromArgb(255, 224, 208, 238),
        };
        TierLabel.Foreground = new SolidColorBrush(tierLabel switch
        {
            "Gold" => Windows.UI.Color.FromArgb(255, 160, 120, 0),
            "Silver" => Windows.UI.Color.FromArgb(255, 90, 90, 110),
            "Bronze" => Windows.UI.Color.FromArgb(255, 140, 80, 20),
            _ => Windows.UI.Color.FromArgb(255, 100, 60, 130),
        });

        ActionButton.Content = viewModel.ActionLabel;
        ActionButton.IsEnabled = viewModel.CanTakeTest;
        ActionButton.Opacity = viewModel.CanTakeTest ? 1.0 : 0.4;
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel.CanTakeTest)
            TakeTestRequested?.Invoke(this, viewModel.TestId);
    }
}