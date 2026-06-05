using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.App.Services.TI;
using UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public class TiMainTestViewModel : INotifyPropertyChanged
{
    private readonly ITiTestService testService;
    private bool isLoading;
    private TiTestCardViewModel? selectedTest;

    public event PropertyChangedEventHandler? PropertyChanged;

    public TiMainTestViewModel(ITiTestService testService)
    {
        this.testService = testService;
    }

    public ObservableCollection<TiTestCardViewModel> Tests { get; } = new();

    public bool IsLoading
    {
        get => isLoading;
        set { isLoading = value; Notify(); Notify(nameof(NoTestsVisible)); }
    }

    public TiTestCardViewModel? SelectedTest
    {
        get => selectedTest;
        set
        {
            if (selectedTest != null) selectedTest.IsSelected = false;
            selectedTest = value;
            if (selectedTest != null) selectedTest.IsSelected = true;
            Notify();
        }
    }

    public Visibility NoTestsVisible =>
        !IsLoading && Tests.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    public async Task LoadTestsAsync()
    {
        IsLoading = true;
        Tests.Clear();

        var tests = await testService.GetAllAsync();
        var session = App.Services.GetRequiredService<SessionContext>();
        int userId = session.UserId;

        foreach (var test in tests)
        {
            bool completed = false;
            try
            {
                completed = await testService.AttemptExistsAsync(userId, test.Id);
            }
            catch
            {
                completed = false;
            }
            var card = new TiTestCardViewModel
            {
                TestId = test.Id,
                Title = test.Title,
                Category = test.Category,
                QuestionTypeLabel = test.QuestionTypeLabel,
                CreatedAt = test.CreatedAt,
                HasBeenTaken = completed
            };
            Tests.Add(card);
        }

        IsLoading = false;
        Notify(nameof(NoTestsVisible));
    }

    private void Notify([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
