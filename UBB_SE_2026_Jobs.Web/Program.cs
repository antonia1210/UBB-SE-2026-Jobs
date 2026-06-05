using Microsoft.AspNetCore.Authentication.Cookies;
using UBB_SE_2026_Jobs.Library.Services.ChatService;
using UBB_SE_2026_Jobs.Library.Services.CompatibilityService;
using UBB_SE_2026_Jobs.Library.Services.FileStorage;
using UBB_SE_2026_Jobs.Library.Services.Developers;
using UBB_SE_2026_Jobs.Library.Services.ImageStorage;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;
using UBB_SE_2026_Jobs.Library.Services.CooldownService;
using UBB_SE_2026_Jobs.Library.Services.CompanyRecommendationService;
using UBB_SE_2026_Jobs.Library.Services.CompanyStatusService;
using UBB_SE_2026_Jobs.Library.Services.CompletenessService;
using UBB_SE_2026_Jobs.Library.Services.Documents;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.JobSkills;
using UBB_SE_2026_Jobs.Library.Services.Matches;
using UBB_SE_2026_Jobs.Library.Services.PersonalityTestService;
using UBB_SE_2026_Jobs.Library.Services.Preferences;
using UBB_SE_2026_Jobs.Library.Services.Recommendations;
using UBB_SE_2026_Jobs.Library.Services.Skills;
using UBB_SE_2026_Jobs.Library.Services.SkillTests;
using UBB_SE_2026_Jobs.Library.Services.UserProfileService;
using UBB_SE_2026_Jobs.Library.Services.UserRecommendationService;
using UBB_SE_2026_Jobs.Library.Services.Users;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;
using UBB_SE_2026_Jobs.Library.Services.Auth;
using UBB_SE_2026_Jobs.Library.ServiceProxies;
using UBB_SE_2026_Jobs.Web.Clients;
using UBB_SE_2026_Jobs.Web.Configuration;
using UBB_SE_2026_Jobs.Web.Infrastructure;
using UBB_SE_2026_Jobs.Web.Services;


var builder = WebApplication.CreateBuilder(args);

var apiConfig = builder.Configuration
        .GetSection("Api")
        .Get<ApiConfiguration>()
        ?? throw new InvalidOperationException("Missing 'Api' configuration section in appsettings.json.");

builder.Services.AddSingleton(apiConfig);
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtForwardingHandler>();

builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
    options.Filters.Add<JwtSessionFilter>();
    options.Filters.Add<ModeAuthorizeFilter>();
})
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddScoped<ModeAuthorizeFilter>();
builder.Services.AddScoped<JwtSessionFilter>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CandidateOnly",   p => p.RequireRole("Candidate"));
    options.AddPolicy("RecruiterOnly",   p => p.RequireRole("Recruiter"));
    options.AddPolicy("AdminOnly",       p => p.RequireRole("Admin"));
    options.AddPolicy("RecruiterOrAdmin",p => p.RequireRole("Recruiter", "Admin"));
    options.AddPolicy("CandidateOrAdmin",p => p.RequireRole("Candidate", "Admin"));
    options.AddPolicy("AuthenticatedUser",p => p.RequireAuthenticatedUser());
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

RegisterServiceProxy<IAuthService, AuthServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ICompletenessService, CompletenessServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IChatService, ChatServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ILocalFileStorageService, FileStorageServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IDeveloperService, DeveloperServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IImageStorageService, ImageStorageServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ICompanyService, CompanyServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ICompatibilityService, CompatibilityServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ICooldownService, CooldownServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ICompanyRecommendationService, CompanyRecommendationServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ICompanyStatusService, CompanyStatusServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IDocumentService, DocumentServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IJobService, JobServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IJobSkillService, JobSkillServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IMatchService, MatchServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IPersonalityTestService, PersonalityTestServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IPreferenceService, PreferenceServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IRecommendationService, RecommendationServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ISkillService, SkillServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<ISkillTestService, SkillTestServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IUserProfileService, UserProfileServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IUserRecommendationService, UserRecommendationServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IUserService, UserServiceProxy>(builder.Services, apiConfig);
RegisterServiceProxy<IUserStatusService, UserStatusServiceProxy>(builder.Services, apiConfig);

builder.Services.AddHttpClient<IUserSkillService, UserSkillServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiConfig.BaseUrl);
}).AddHttpMessageHandler<JwtForwardingHandler>();

// Tests & Interviews API clients
builder.Services.AddHttpClient<ITiAuthService, TiAuthService>(c => c.BaseAddress = new Uri(apiConfig.BaseUrl));
RegisterApiClient<TestsApiClient>(builder.Services, apiConfig);
RegisterApiClient<JobsApiClient>(builder.Services, apiConfig);
RegisterApiClient<ApplicantsApiClient>(builder.Services, apiConfig);
RegisterApiClient<QuestionsApiClient>(builder.Services, apiConfig);
RegisterApiClient<AnswersApiClient>(builder.Services, apiConfig);
RegisterApiClient<SlotsApiClient>(builder.Services, apiConfig);
RegisterApiClient<LeaderboardApiClient>(builder.Services, apiConfig);
RegisterApiClient<TestAttemptsApiClient>(builder.Services, apiConfig);
RegisterApiClient<EventsApiClient>(builder.Services, apiConfig);
RegisterApiClient<PaymentApiClient>(builder.Services, apiConfig);
RegisterApiClient<InterviewSessionsApiClient>(builder.Services, apiConfig);
RegisterApiClient<UsersApiClient>(builder.Services, apiConfig);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

static void RegisterServiceProxy<TService, TProxy>(
    IServiceCollection services,
    ApiConfiguration apiConfiguration)
    where TService : class
    where TProxy : class, TService
{
    services.AddHttpClient<TService, TProxy>(client =>
        client.BaseAddress = new Uri(apiConfiguration.BaseUrl))
        .AddHttpMessageHandler<JwtForwardingHandler>();
}

static void RegisterApiClient<TClient>(
    IServiceCollection services,
    ApiConfiguration apiConfiguration)
    where TClient : class
{
    services.AddHttpClient<TClient>(client =>
        client.BaseAddress = new Uri(apiConfiguration.BaseUrl))
        .AddHttpMessageHandler<JwtForwardingHandler>();
}
