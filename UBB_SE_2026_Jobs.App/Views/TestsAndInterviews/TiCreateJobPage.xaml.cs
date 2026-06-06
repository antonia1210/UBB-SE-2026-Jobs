using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.App.ViewModels.TI;
using UBB_SE_2026_Jobs.App;

namespace UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

public sealed partial class TiCreateJobPage : Page
{
    public TiCreateJobViewModel ViewModel { get; }

    public TiCreateJobPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TiCreateJobViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.InitializeAsync();

        if (e.Parameter is TiJobPostingDto job)
        {
            ViewModel.LoadForEdit(job);
            PageTitleText.Text = "Edit Job";
            SaveButton.Content = "Save Changes";

            SelectComboByTag(IndustryCombo, job.IndustryField);
            SelectComboByTag(JobTypeCombo, job.JobType);
            SelectComboByTag(ExpLevelCombo, job.ExperienceLevel);
        }
    }

    private static void SelectComboByTag(ComboBox combo, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        foreach (var item in combo.Items)
        {
            if (item is ComboBoxItem comboItem && comboItem.Tag as string == value)
            {
                combo.SelectedItem = comboItem;
                return;
            }
        }
    }

    private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox combo) return;
        var value = (combo.SelectedItem as ComboBoxItem)?.Tag as string ?? string.Empty;

        if (combo == IndustryCombo) ViewModel.IndustryField = value;
        else if (combo == JobTypeCombo) ViewModel.JobType = value;
        else if (combo == ExpLevelCombo) ViewModel.ExperienceLevel = value;
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.SaveJobAsync();
        if (ViewModel.SavedSuccessfully)
            Frame.Navigate(typeof(TiJobsPage));
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Frame.GoBack();
}
