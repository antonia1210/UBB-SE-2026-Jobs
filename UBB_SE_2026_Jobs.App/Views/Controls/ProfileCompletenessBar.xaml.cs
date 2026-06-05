using Microsoft.UI.Xaml.Controls;

namespace UBB_SE_2026_Jobs.App.Views.Controls;

public sealed partial class ProfileCompletenessBar : UserControl
{
    public ProfileCompletenessBar()
    {
        InitializeComponent();
    }

    public void Update(int percentage, string promptText)
    {
        barProgress.Value = percentage;
        percentageLabel.Text = $"{percentage}%";
        promptLabel.Text = promptText;
    }
}
