using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services.PersonalityTestService;

namespace UBB_SE_2026_Jobs.App.ViewModels;

public partial class PersonalityTestViewModel : DispatchableObservableObject
{
    private const int NumberOfTopRolesToDisplay = 3;

    private readonly IPersonalityTestService personalityTestService;
    private readonly SessionContext session;
    private IReadOnlyDictionary<PersonalityQuestion, AnswerValue> lastAnswers = new Dictionary<PersonalityQuestion, AnswerValue>();

    private List<RoleResultViewModel> topRoles = new();
    private RoleResultViewModel? selectedRole;
    private string? saveMessage;
    private bool isTestSubmitted;

    public PersonalityTestViewModel(SessionContext session, IPersonalityTestService personalityTestService)
    {
        this.session = session;
        this.personalityTestService = personalityTestService;

        Questions = PersonalityTestService.LoadQuestions()
            .Select(question => new QuestionViewModel(question))
            .ToList();

        foreach (var questionViewModel in Questions)
        {
            questionViewModel.PropertyChanged += (_, eventArguments) =>
            {
                if (eventArguments.PropertyName == nameof(QuestionViewModel.SelectedAnswer))
                {
                    SubmitCommand.NotifyCanExecuteChanged();
                }
            };
        }
    }

    public List<QuestionViewModel> Questions { get; }

    public List<RoleResultViewModel> TopRoles
    {
        get => topRoles;
        set => SetProperty(ref topRoles, value);
    }

    public RoleResultViewModel? SelectedRole
    {
        get => selectedRole;
        set
        {
            if (SetProperty(ref selectedRole, value))
            {
                SaveResultCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string? SaveMessage
    {
        get => saveMessage;
        set => SetProperty(ref saveMessage, value);
    }

    public bool IsTestSubmitted
    {
        get => isTestSubmitted;
        set => SetProperty(ref isTestSubmitted, value);
    }

    public bool CanSubmit => Questions.All(question => question.IsAnswered);
    public bool CanSave => SelectedRole is not null;
    private bool hasExistingResult;
    private JobRole? existingRole;

    public JobRole? ExistingRole
    {
        get => existingRole;
        set => SetProperty(ref existingRole, value);
    }

    public bool HasExistingResult
    {
        get => hasExistingResult;
        set => SetProperty(ref hasExistingResult, value);
    }

    public async Task LoadExistingResultAsync(CancellationToken cancellationToken = default)
    {
        var result = await personalityTestService.GetByUserIdAsync(
            ViewModelSupport.ResolveUserId(session), cancellationToken);

        if (result is not null)
        {
            ExistingRole = result.SelectedRole;
            HasExistingResult = true;
        }
    }

    [RelayCommand]
    private void TakeTest()
    {
        HasExistingResult = false;
    }

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private void Submit()
    {
        lastAnswers = CollectAnswersFromQuestions();
        var traitScores = personalityTestService.CalculateTraitScores(lastAnswers);
        var roleScores = personalityTestService.CalculateRoleScores(traitScores);
        var topRolesDictionary = personalityTestService.GetTopRoles(roleScores, NumberOfTopRolesToDisplay);

        TopRoles = topRolesDictionary
            .Select(rolePair => new RoleResultViewModel(rolePair.Key, rolePair.Value))
            .ToList();

        IsTestSubmitted = true;
    }

    [RelayCommand]
    private void SelectRole(RoleResultViewModel roleResultViewModel)
    {
        foreach (var topRoleViewModel in TopRoles)
        {
            topRoleViewModel.IsSelected = false;
        }

        roleResultViewModel.IsSelected = true;
        SelectedRole = roleResultViewModel;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveResultAsync(CancellationToken cancellationToken)
    {
        if (SelectedRole is null)
        {
            return;
        }

        if (lastAnswers.Count == 0)
        {
            lastAnswers = CollectAnswersFromQuestions();
        }

        await personalityTestService
            .SaveResultAsync(ViewModelSupport.ResolveUserId(session), lastAnswers, SelectedRole.Role, cancellationToken)
            ;

        SaveMessage = $"Your personality test result has been updated to {SelectedRole.DisplayName}.";
    }

    private Dictionary<PersonalityQuestion, AnswerValue> CollectAnswersFromQuestions()
    {
        var answers = new Dictionary<PersonalityQuestion, AnswerValue>();
        foreach (var questionViewModel in Questions)
        {
            if (questionViewModel.SelectedAnswer is int selectedAnswer)
            {
                answers[questionViewModel.Question] = (AnswerValue)selectedAnswer;
            }
        }

        return answers;
    }
}
