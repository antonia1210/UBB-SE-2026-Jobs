using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.App.Views.Auth;
using UBB_SE_2026_Jobs.App.Views.Candidate;
using UBB_SE_2026_Jobs.App.Views.TestsAndInterviews;

namespace UBB_SE_2026_Jobs.App;

public sealed partial class MainWindow : Window
{
    private static readonly HashSet<string> CandidatePages =
    [
        "UserRecommendationPage",
        "UserStatusPage",
        "UserProfilePage",
        "TestDashboardPage",
        "PersonalityTestPage",
        "CompatibilityOverviewPage",
        "DocumentsPage",
        "TiInterviewSlotsPage",
    ];

    private static readonly HashSet<string> CompanyPages =
    [
        "CompanyRecommendationPage",
        "CompanyStatusPage",
        "TiRecruiterInterviewsPage",
        "TiMainTestPage",
    ];

    private static readonly HashSet<string> DeveloperPages =
    [
        "DeveloperPage",
    ];

    private static readonly HashSet<string> SharedPages =
    [
        "ChatPage",
        "TiJobsPage",
        "TiEventsPage",
    ];

    private static readonly Dictionary<string, Type> PageMap = new()
    {
        ["UserRecommendationPage"]      = typeof(UserRecommendationPage),
        ["UserStatusPage"]              = typeof(UserStatusPage),
        ["UserProfilePage"]             = typeof(UserProfilePage),
        ["ProfileFormPage"]             = typeof(ProfileFormPage),
        ["TestDashboardPage"]           = typeof(TestDashboardPage),
        ["PersonalityTestPage"]         = typeof(PersonalityTestPage),
        ["CompatibilityOverviewPage"]   = typeof(CompatibilityOverviewPage),
        ["DocumentsPage"]               = typeof(DocumentsPage),
        ["ExportCVPage"]                = typeof(ExportCVPage),
        ["CompanyRecommendationPage"]   = typeof(Views.Company.CompanyRecommendationPage),
        ["CompanyStatusPage"]           = typeof(Views.Company.CompanyStatusPage),
        ["DeveloperPage"]               = typeof(Views.Developer.DeveloperPage),
        ["ChatPage"]                    = typeof(Views.ChatPage),
        ["LoginPage"]                   = typeof(LoginPage),
        ["RegisterPage"]                = typeof(RegisterPage),
        // TI (Tests & Interviews) pages
        ["TiMainTestPage"]              = typeof(TiMainTestPage),
        ["TiEventsPage"]                = typeof(TiEventsPage),
        ["TiJobsPage"]                  = typeof(TiJobsPage),
        ["TiInterviewSlotsPage"]        = typeof(TiInterviewSlotsPage),
        ["TiRecruiterInterviewsPage"]   = typeof(TiRecruiterInterviewsPage),
        ["TiLeaderboardPage"]           = typeof(TiLeaderboardPage),
    };

    public Frame NavigationFrame => contentFrame;

    public MainWindow()
    {
        InitializeComponent();
        contentFrame.Navigated += ContentFrame_Navigated;
        var session = App.Services.GetRequiredService<SessionContext>();
        if (session.IsAuthenticated)
        {
            ShowAuthenticatedShell();
        }
        else
        {
            ShowLogin();
        }
    }

    public void ShowAuthenticatedShell()
    {
        var session = App.Services.GetRequiredService<SessionContext>();
        if (!session.IsAuthenticated)
        {
            ShowLogin();
            return;
        }

        navView.IsPaneVisible = true;
        navView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        navView.IsBackEnabled = false;
        accountSummary.Visibility = navView.IsPaneOpen ? Visibility.Visible : Visibility.Collapsed;

        UpdateModeVisibility();
        UpdateAccountSummary();
        var defaultPage = GetDefaultPage(session.Mode);
        NavigateTo(defaultPage);
        UpdateNavSelection(defaultPage);
    }

    public void ShowLogin()
    {
        navView.IsPaneVisible = false;
        navView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        navView.SelectedItem = null;
        accountSummary.Visibility = Visibility.Collapsed;
        contentFrame.BackStack.Clear();
        contentFrame.Navigate(typeof(LoginPage));
    }

    public void ShowRegister()
    {
        navView.IsPaneVisible = false;
        navView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        navView.SelectedItem = null;
        accountSummary.Visibility = Visibility.Collapsed;
        contentFrame.BackStack.Clear();
        contentFrame.Navigate(typeof(RegisterPage));
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs eventArguments)
    {
        if (eventArguments.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            if (tag == "Logout")
            {
                App.Services.GetRequiredService<SessionContext>().SignOut();
                ShowLogin();
                return;
            }

            NavigateTo(tag);
        }
    }

    private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs eventArguments)
    {
        if (contentFrame.CanGoBack)
            contentFrame.GoBack();
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs eventArguments)
    {
        navView.IsBackEnabled = false;
    }

    private void NavView_PaneOpened(NavigationView sender, object args)
    {
        accountSummary.Visibility = App.Services.GetRequiredService<SessionContext>().IsAuthenticated
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void NavView_PaneClosed(NavigationView sender, object args)
        => accountSummary.Visibility = Visibility.Collapsed;

    private void UpdateModeVisibility()
    {
        var session = App.Services.GetRequiredService<SessionContext>();
        foreach (var item in navView.MenuItems)
        {
            if (item is not NavigationViewItem navigationViewItem || navigationViewItem.Tag is not string tag)
            {
                continue;
            }

            var visible = SharedPages.Contains(tag)
                || session.Mode == AppMode.Candidate && CandidatePages.Contains(tag)
                || session.Mode == AppMode.Company && CompanyPages.Contains(tag)
                || session.Mode == AppMode.Developer && DeveloperPages.Contains(tag);

            navigationViewItem.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void UpdateAccountSummary()
    {
        var session = App.Services.GetRequiredService<SessionContext>();
        accountNameText.Text = string.IsNullOrWhiteSpace(session.DisplayName)
            ? session.Email
            : session.DisplayName;
        accountModeText.Text = session.Mode == AppMode.Company ? "Recruiter" : "Candidate";
    }

    private void NavigateTo(string tag)
    {
        var session = App.Services.GetRequiredService<SessionContext>();
        if (!session.IsAuthenticated && tag != "LoginPage" && tag != "RegisterPage")
        {
            ShowLogin();
            return;
        }

        if (PageMap.TryGetValue(tag, out var pageType))
            contentFrame.Navigate(pageType);
    }

    private void UpdateNavSelection(string tag)
    {
        foreach (var item in navView.MenuItems)
        {
            if (item is NavigationViewItem navigationViewItem && navigationViewItem.Tag as string == tag)
            {
                navView.SelectedItem = navigationViewItem;
                return;
            }
        }

        foreach (var item in navView.FooterMenuItems)
        {
            if (item is NavigationViewItem navigationViewItem && navigationViewItem.Tag as string == tag)
            {
                navView.SelectedItem = navigationViewItem;
                return;
            }
        }
    }

    private static string GetDefaultPage(AppMode mode)
    {
        return mode switch
        {
            AppMode.Company => "CompanyRecommendationPage",
            AppMode.Developer => "DeveloperPage",
            _ => "UserRecommendationPage",
        };
    }
}
