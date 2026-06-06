using CommunityToolkit.Mvvm.Input;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.ServiceProxies.TI;
using UBB_SE_2026_Jobs.Library.Services.Auth;

namespace UBB_SE_2026_Jobs.App.ViewModels.Auth;

public partial class LoginViewModel : DispatchableObservableObject
{
    private readonly IAuthService authService;
    private readonly ITiAuthService tiAuthService;
    private readonly SessionContext session;

    private string email = string.Empty;
    private string password = string.Empty;
    private string errorMessage = string.Empty;
    private bool isBusy;

    public LoginViewModel(IAuthService authService, ITiAuthService tiAuthService, SessionContext session)
    {
        this.authService = authService;
        this.tiAuthService = tiAuthService;
        this.session = session;
    }

    public event Action? LoginSucceeded;

    public string Email
    {
        get => email;
        set => SetProperty(ref email, value);
    }

    public string Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }

    public string ErrorMessage
    {
        get => errorMessage;
        set => SetProperty(ref errorMessage, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }

    [RelayCommand]
    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Enter your email and password.";
            return;
        }

        try
        {
            IsBusy = true;
            var result = await authService.LoginAsync(Email.Trim(), Password, cancellationToken);
            if (!result.Succeeded || result.Response is null)
            {
                ErrorMessage = "Invalid email or password.";
                return;
            }

            session.SignIn(result.Response);
            var accountInfo = await tiAuthService.LoginAsync(Email.Trim(), Password);
            session.ApplyAccountRole(accountInfo?.Role, accountInfo?.CompanyId);
            LoginSucceeded?.Invoke();
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Login failed: {exception.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
