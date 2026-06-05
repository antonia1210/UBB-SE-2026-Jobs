using CommunityToolkit.Mvvm.ComponentModel;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.CompatibilityService;

namespace UBB_SE_2026_Jobs.App.ViewModels;

public class CompatibilityOverviewViewModel : DispatchableObservableObject
{
    private readonly ICompatibilityService compatibilityService;
    private readonly SessionContext session;
    private List<RoleResult> roleResults = new();
    private RoleResult? selectedResult;
    private string errorMessage = string.Empty;
    private bool isLoading;

    public CompatibilityOverviewViewModel(ICompatibilityService compatibilityService, SessionContext session)
    {
        this.compatibilityService = compatibilityService;
        this.session = session;
    }

    public List<RoleResult> RoleResults
    {
        get => roleResults;
        private set => SetProperty(ref roleResults, value);
    }

    public RoleResult? SelectedResult
    {
        get => selectedResult;
        private set => SetProperty(ref selectedResult, value);
    }

    public string ErrorMessage
    {
        get => errorMessage;
        private set => SetProperty(ref errorMessage, value);
    }

    public bool IsLoading
    {
        get => isLoading;
        private set => SetProperty(ref isLoading, value);
    }

    public async Task LoadAllRolesAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        try
        {
            RoleResults = (await compatibilityService
                .CalculateAllAsync(ViewModelSupport.ResolveUserId(session), cancellationToken)
                ).ToList();
            ErrorMessage = string.Empty;
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public List<RoleResult> GetRoleResults() => RoleResults;

    public RoleResult? GetResultForRole(JobRole role)
    {
        return RoleResults.FirstOrDefault(result => result.JobRole == role);
    }

    public void OnRoleSelected(JobRole role)
    {
        SelectedResult = GetResultForRole(role);
    }

    public RoleResult? GetSelectedResult() => SelectedResult;
    public string GetErrorMessage() => ErrorMessage;
}
