using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using UBB_SE_2026_Jobs.Library.Persistence;
using UBB_SE_2026_Jobs.Library.Repositories;
using UBB_SE_2026_Jobs.Library.Repositories.Chats;
using UBB_SE_2026_Jobs.Library.Repositories.Companies;
using UBB_SE_2026_Jobs.Library.Repositories.Documents;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Repositories.Jobs;
using UBB_SE_2026_Jobs.Library.Repositories.Matches;
using UBB_SE_2026_Jobs.Library.Repositories.Messages;
using UBB_SE_2026_Jobs.Library.Repositories.PersonalityTests;
using UBB_SE_2026_Jobs.Library.Repositories.Recommendations;
using UBB_SE_2026_Jobs.Library.Repositories.Skills;
using UBB_SE_2026_Jobs.Library.Repositories.Users;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.ChatService;
using UBB_SE_2026_Jobs.Library.Services.CompanyRecommendationService;
using UBB_SE_2026_Jobs.Library.Services.PussyCatsCompanyService;
using UBB_SE_2026_Jobs.Library.Services.CompanyStatusService;
using UBB_SE_2026_Jobs.Library.Services.CompatibilityService;
using UBB_SE_2026_Jobs.Library.Services.CompletenessService;
using UBB_SE_2026_Jobs.Library.Services.CooldownService;
using UBB_SE_2026_Jobs.Library.Services.CvParsing;
using UBB_SE_2026_Jobs.Library.Services.Developers;
using UBB_SE_2026_Jobs.Library.Services.Documents;
using UBB_SE_2026_Jobs.Library.Services.FileStorage;
using UBB_SE_2026_Jobs.Library.Services.ImageStorage;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.JobSkills;
using UBB_SE_2026_Jobs.Library.Services.Matches;
using UBB_SE_2026_Jobs.Library.Services.PdfExport;
using UBB_SE_2026_Jobs.Library.Services.PersonalityTestService;
using UBB_SE_2026_Jobs.Library.Services.Preferences;
using UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;
using UBB_SE_2026_Jobs.Library.Services.Recommendations;
using UBB_SE_2026_Jobs.Library.Services.SkillGapService;
using UBB_SE_2026_Jobs.Library.Services.Skills;
using UBB_SE_2026_Jobs.Library.Services.SkillTests;
using UBB_SE_2026_Jobs.Library.Services.UserProfileService;
using UBB_SE_2026_Jobs.Library.Services.UserRecommendationService;
using UBB_SE_2026_Jobs.Library.Services.Users;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;
using UBB_SE_2026_Jobs.Library.Services.UserRecommendations;
using UBB_SE_2026_Jobs.Library.Services.Completeness;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASecretKeyForJwtAuthenticationMustBeLongEnough12345!";
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "UBB_SE_2026_Jobs.Api",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "UBB_SE_2026_Jobs.Clients",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

builder.Services.AddAuthorization();

// Database Configuration
builder.Services.AddDbContext<JobsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("JobsDb")));

// --- REPOSITORIES ---

// PussyCats Repositories (Renamed)
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IRecruiterRepository, RecruiterRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPussyCatsJobRepository, PussyCatsJobRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IPussyCatsCompanyRepository, PussyCatsCompanyRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IJobSkillRepository, JobSkillRepository>();
builder.Services.AddScoped<IUserSkillRepository, UserSkillRepository>();
builder.Services.AddScoped<ISkillGroupRepository, SkillGroupRepository>();
builder.Services.AddScoped<IPersonalityTestRepository, PersonalityTestRepository>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();

// TestsAndInterviews Repositories (Renamed)
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IApplicantRepository, ApplicantRepository>();
builder.Services.AddScoped<ICollaboratorsRepo, CollaboratorsRepo>();
builder.Services.AddScoped<ITestsCompanyRepository, TestsCompanyRepository>();
builder.Services.AddScoped<IEventsRepo, EventsRepo>();
builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<ITestsJobsRepository, TestsJobsRepository>();
builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ISlotRepository, SlotRepository>();
builder.Services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
builder.Services.AddScoped<ITestRepository, TestRepository>();

// --- SERVICES ---

// PussyCats Services (Renamed)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPussyCatsJobService, PussyCatsJobService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<IDocumentService>(provider => provider.GetRequiredService<DocumentService>());
builder.Services.AddScoped<ILocalDocumentFileService>(provider => provider.GetRequiredService<DocumentService>());
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IPersonalityTestService, PersonalityTestService>();
builder.Services.AddScoped<ICvParsingService, CvParsingService>();
builder.Services.AddScoped<ICompletenessService, CompletenessService>();
builder.Services.AddScoped<ISkillTestService, SkillTestService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IRecommendationAlgorithm, RecommendationAlgorithm>();
builder.Services.AddScoped<IUserRecommendationService, UserRecommendationService>();
builder.Services.AddScoped<ICooldownService>(provider =>
    new CooldownService(
        provider.GetRequiredService<IRecommendationRepository>(),
        TimeSpan.FromHours(24)
    ));
builder.Services.AddScoped<IUserSkillService, UserSkillService>();
builder.Services.AddScoped<ISkillGapService, SkillGapService>();
builder.Services.AddScoped<ICompatibilityService, CompatibilityService>();
builder.Services.AddScoped<IUserStatusService, UserStatusService>();
builder.Services.AddSingleton<IDeveloperService, DeveloperService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IPussyCatsCompanyService, PussyCatsCompanyService>();
builder.Services.AddTransient<ICompanyRecommendationService, CompanyRecommendationService>();
builder.Services.AddScoped<ICompanyStatusService, CompanyStatusService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IJobSkillService, JobSkillService>();

// TestsAndInterviews Services (Renamed)
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IApplicantService, ApplicantService>();
builder.Services.AddScoped<IAttemptValidationService, AttemptValidationService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICollaboratorsService, CollaboratorsService>();
builder.Services.AddScoped<ITestsCompanyService, TestsCompanyService>();
builder.Services.AddScoped<IDataProcessingService, DataProcessingService>();
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGradingService, GradingService>();
builder.Services.AddScoped<IInterviewSessionService, InterviewSessionService>();
builder.Services.AddScoped<ITestsJobsService, TestsJobsService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProfileCompletionCalculator, ProfileCompletionCalculator>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<ITestAttemptService, TestAttemptService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<ITimerService, TimerService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ICompanyStatsService, CompanyStatsService>();
builder.Services.AddScoped<ITestsAuthService, TestsAuthService>();

// Infrastructure
builder.Services.AddSingleton<IPdfExportService>(serviceProvider =>
{
    var webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var templatePath = Path.Combine(webHostEnvironment.WebRootPath, "templates", "CVHtmlTemplate.html");
    var templateHtml = File.ReadAllText(templatePath);
    return new PdfExportService(templateHtml);
});

builder.Services.AddSingleton<ILocalFileStorageService>(serviceProvider =>
{
    var uploadsPath = Path.Combine(AppContext.BaseDirectory, "uploads", "files");
    return new LocalFileStorageService(uploadsPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<JobsDbContext>();
    database.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
