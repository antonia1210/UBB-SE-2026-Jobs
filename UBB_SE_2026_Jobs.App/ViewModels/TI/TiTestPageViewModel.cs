using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.UI.Xaml;
using UBB_SE_2026_Jobs.Library.DTOs.TI;
using UBB_SE_2026_Jobs.Library.ServiceProxies.TI;

namespace UBB_SE_2026_Jobs.App.ViewModels.TI;

public class TiTestPageViewModel : INotifyPropertyChanged
{
    private readonly ITiTestService testService;
    private string testTitle = string.Empty;
    private TimeSpan timeLeft = TimeSpan.FromMinutes(30);
    private DispatcherTimer? timer;
    private int answeredCount;

    public event PropertyChangedEventHandler? PropertyChanged;

    public TiTestPageViewModel(ITiTestService testService)
    {
        this.testService = testService;
    }

    public ObservableCollection<TiQuestionViewModel> Questions { get; } = new();

    public string TestTitle
    {
        get => testTitle;
        set { testTitle = value; Notify(); }
    }

    public string TimerDisplay => timeLeft.ToString(@"mm\:ss");
    public Action? OnTimerExpired { get; set; }

    public int AnsweredCount
    {
        get => answeredCount;
        set { answeredCount = value; Notify(); }
    }

    public int TotalCount => Questions.Count;
    public int UserId { get; set; }
    public int TestId { get; set; }

    public async Task LoadAsync(int testId, int userId)
    {
        TestId = testId;
        UserId = userId;

        var test = await testService.GetByIdAsync(testId);
        if (test == null) return;

        TestTitle = test.Title;

        // Tests are replayable. Resume the active IN_PROGRESS attempt if one exists,
        // otherwise start a fresh one. Never block based on prior completed attempts.
        var activeAttempt = await testService.GetAttemptByUserAndTestAsync(userId, testId);
        if (activeAttempt == null)
        {
            try { await testService.StartAttemptAsync(userId, testId); }
            catch { }
        }

        var questions = await testService.GetQuestionsByTestIdAsync(testId);
        int index = 1;
        foreach (var question in questions)
        {
            var type = question.QuestionType switch
            {
                "SINGLE_CHOICE" => TiQuestionType.SINGLE_CHOICE,
                "MULTIPLE_CHOICE" => TiQuestionType.MULTIPLE_CHOICE,
                "TRUE_FALSE" => TiQuestionType.TRUE_FALSE,
                "INTERVIEW" => TiQuestionType.INTERVIEW,
                _ => TiQuestionType.TEXT
            };

            if (type == TiQuestionType.INTERVIEW) continue;

            var questionViewModel = new TiQuestionViewModel
            {
                QuestionId = question.Id,
                DisplayNumber = index++,
                QuestionText = question.QuestionText,
                Type = type,
            };

            if (type == TiQuestionType.SINGLE_CHOICE || type == TiQuestionType.MULTIPLE_CHOICE)
            {
                var options = new List<string>();
                if (!string.IsNullOrEmpty(question.OptionsJson))
                {
                    try { options = JsonSerializer.Deserialize<List<string>>(question.OptionsJson) ?? new(); }
                    catch { }
                }

                for (int optionIndex = 0; optionIndex < options.Count; optionIndex++)
                {
                    questionViewModel.Options.Add(new TiOptionViewModel
                    {
                        Text = options[optionIndex],
                        Index = optionIndex,
                        GroupName = $"q_{question.Id}",
                        OnSelectionChanged = UpdateAnsweredCount,
                    });
                }
            }

            questionViewModel.OnAnswerChanged = UpdateAnsweredCount;
            Questions.Add(questionViewModel);
        }

        Notify(nameof(TotalCount));
        StartTimer();
    }

    public void StopTimer() => timer?.Stop();

    public async Task<float> SubmitAsync()
    {
        StopTimer();
        var answers = Questions
            .Select(question => new TiAnswerDto { QuestionId = question.QuestionId, Value = question.GetAnswerValue() })
            .Where(answer => !string.IsNullOrEmpty(answer.Value))
            .ToList();

        return await testService.SubmitAttemptAsync(UserId, TestId, answers);
    }

    private void StartTimer()
    {
        timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += (_, _) =>
        {
            timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
            Notify(nameof(TimerDisplay));
            if (timeLeft <= TimeSpan.Zero)
            {
                timer.Stop();
                OnTimerExpired?.Invoke();
            }
        };
        timer.Start();
    }

    private void UpdateAnsweredCount() =>
        AnsweredCount = Questions.Count(question => question.IsAnswered());

    private void Notify([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
