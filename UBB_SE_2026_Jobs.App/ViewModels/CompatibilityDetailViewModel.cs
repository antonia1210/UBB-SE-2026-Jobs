using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.App.ViewModels;

public class CompatibilityDetailViewModel : DispatchableObservableObject
{
    private RoleResult? currentRoleResult;
    private string errorMessage = string.Empty;

    public RoleResult? CurrentRoleResult
    {
        get => currentRoleResult;
        private set => SetProperty(ref currentRoleResult, value);
    }

    public string ErrorMessage
    {
        get => errorMessage;
        private set => SetProperty(ref errorMessage, value);
    }

    public void LoadResult(RoleResult result)
    {
        CurrentRoleResult = result;
        ErrorMessage = string.Empty;
    }

    public double GetMatchScore() => CurrentRoleResult?.MatchScore ?? 0;

    public string GetRoleName()
    {
        return CurrentRoleResult is null
            ? string.Empty
            : ViewModelSupport.FormatJobRole(CurrentRoleResult.JobRole);
    }

    public List<Suggestion> GetSuggestions() => CurrentRoleResult?.Suggestions ?? new List<Suggestion>();
    public List<CompatibilitySkillScore> GetSkillScores() => CurrentRoleResult?.SkillScores ?? new List<CompatibilitySkillScore>();
    public string GetErrorMessage() => ErrorMessage;
}
