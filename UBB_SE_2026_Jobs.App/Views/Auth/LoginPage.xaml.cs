using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UBB_SE_2026_Jobs.App.ViewModels.Auth;

namespace UBB_SE_2026_Jobs.App.Views.Auth;

public sealed partial class LoginPage : Page
{
    private readonly LoginViewModel viewModel;

    public LoginPage()
    {
        InitializeComponent();
        viewModel = App.Services.GetRequiredService<LoginViewModel>();
        viewModel.LoginSucceeded += OnLoginSucceeded;
        DataContext = viewModel;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs eventArguments)
    {
        viewModel.Password = PasswordBox.Password;
    }

    private void NavigateToRegister_Click(object sender, RoutedEventArgs eventArguments)
    {
        if (App.MainAppWindow is MainWindow mainWindow)
        {
            mainWindow.ShowRegister();
        }
    }

    private void OnLoginSucceeded()
    {
        if (App.MainAppWindow is MainWindow mainWindow)
        {
            mainWindow.ShowAuthenticatedShell();
        }
    }
}
