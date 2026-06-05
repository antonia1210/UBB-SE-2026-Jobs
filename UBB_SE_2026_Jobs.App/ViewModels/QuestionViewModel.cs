using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.App.ViewModels;

public class QuestionViewModel : DispatchableObservableObject
{
    private int? selectedAnswer;

    public QuestionViewModel(PersonalityQuestion question)
    {
        Question = question;
    }

    public PersonalityQuestion Question { get; }

    public int? SelectedAnswer
    {
        get => selectedAnswer;
        set
        {
            if (SetProperty(ref selectedAnswer, value))
            {
                OnPropertyChanged(nameof(IsAnswered));
            }
        }
    }

    public bool IsAnswered => SelectedAnswer is not null;
}
