using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using UBB_SE_2026_Jobs.Library.Persistence;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Data;
using PcCompanyService = UBB_SE_2026_Jobs.Library.Services.CompanyService;
using UBB_SE_2026_Jobs.Library.Repositories.Chats;
using UBB_SE_2026_Jobs.Library.Repositories.Companies;
using UBB_SE_2026_Jobs.Library.Repositories.Documents;
using UBB_SE_2026_Jobs.Library.Repositories.Jobs;
using UBB_SE_2026_Jobs.Library.Repositories.Matches;
using UBB_SE_2026_Jobs.Library.Repositories.Messages;
using UBB_SE_2026_Jobs.Library.Repositories.PersonalityTests;
using UBB_SE_2026_Jobs.Library.Repositories.Recommendations;
using UBB_SE_2026_Jobs.Library.Repositories.Skills;
using UBB_SE_2026_Jobs.Library.Repositories.SkillTests;
using UBB_SE_2026_Jobs.Library.Repositories.Users;
using UBB_SE_2026_Jobs.Library.Services.CompatibilityService;
using UBB_SE_2026_Jobs.Library.Services.CooldownService;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;
using UBB_SE_2026_Jobs.Library.Services.CompanyRecommendationService;
using UBB_SE_2026_Jobs.Library.Services.CompanyStatusService;
using UBB_SE_2026_Jobs.Library.Services.CompletenessService;
using UBB_SE_2026_Jobs.Library.Services.Documents;
using UBB_SE_2026_Jobs.Library.Services.CvParsing;
using UBB_SE_2026_Jobs.Library.Services.FileStorage;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.JobSkills;
using UBB_SE_2026_Jobs.Library.Services.Matches;
using UBB_SE_2026_Jobs.Library.Services.PersonalityTestService;
using UBB_SE_2026_Jobs.Library.Services.Preferences;
using UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;
using UBB_SE_2026_Jobs.Library.Services.Recommendations;
using UBB_SE_2026_Jobs.Library.Services.Skills;
using UBB_SE_2026_Jobs.Library.Services.SkillTests;
using UBB_SE_2026_Jobs.Library.Services.SkillGapService;
using UBB_SE_2026_Jobs.Library.Services.UserProfileService;
using UBB_SE_2026_Jobs.Library.Services.UserRecommendationService;
using UBB_SE_2026_Jobs.Library.Services.Users;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;
using UBB_SE_2026_Jobs.Library.Services.ChatService;
using UBB_SE_2026_Jobs.Library.Services.Developers;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;
using UBB_SE_2026_Jobs.Library.Services.PdfExport;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Services;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Missing 'Jwt:Key' configuration.");
}

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "UBB-SE-2026",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "UBB-SE-Client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });
builder.Services.AddAuthorization();

var connString = builder.Configuration.GetConnectionString("JobsDb");

// Primary DbContext (PussyCats / main schema)
builder.Services.AddDbContext<PussyCatsDbContext>(options =>
    options.UseSqlServer(connString));

// Secondary DbContext (Tests & Interviews schema) — migrations live in Library assembly
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connString, b => b.MigrationsAssembly("UBB-SE-2026-Jobs.Library")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebProject", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5238",
            "https://localhost:7087",
            "http://localhost:3000",
            "http://127.0.0.1:5238"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// --- PussyCats repositories ---
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IJobSkillRepository, JobSkillRepository>();
builder.Services.AddScoped<IUserSkillRepository, UserSkillRepository>();
builder.Services.AddScoped<ISkillGroupRepository, SkillGroupRepository>();
builder.Services.AddScoped<ISkillTestRepository, SkillTestRepository>();
builder.Services.AddScoped<IPersonalityTestRepository, PersonalityTestRepository>();
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();

// --- PussyCats services ---
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<IDocumentService>(provider => provider.GetRequiredService<DocumentService>());
builder.Services.AddScoped<ILocalDocumentFileService>(provider => provider.GetRequiredService<DocumentService>());
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IPersonalityTestService, PersonalityTestService>();
builder.Services.AddScoped<ICvParsingService, CvParsingService>();
builder.Services.AddScoped<ICompletenessService, CompletenessService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ISkillTestService, SkillTestService>();
builder.Services.AddScoped<IJobSkillService, JobSkillService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IRecommendationAlgorithm, RecommendationAlgorithm>();
builder.Services.AddScoped<IUserRecommendationService, UserRecommendationService>();
builder.Services.AddScoped<ICooldownService>(provider =>
    new CooldownService(
        provider.GetRequiredService<IRecommendationRepository>(),
        TimeSpan.FromHours(24)));
builder.Services.AddScoped<IUserSkillService, UserSkillService>();
builder.Services.AddScoped<ISkillGapService, SkillGapService>();
builder.Services.AddScoped<ICompatibilityService, CompatibilityService>();
builder.Services.AddScoped<IUserStatusService, UserStatusService>();
builder.Services.AddSingleton<IDeveloperService, DeveloperService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<PcCompanyService.ICompanyService, PcCompanyService.CompanyService>();
builder.Services.AddTransient<ICompanyRecommendationService, CompanyRecommendationService>();
builder.Services.AddScoped<ICompanyStatusService, CompanyStatusService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();

builder.Services.AddSingleton<IPdfExportService>(serviceProvider =>
{
    var webhostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
    var templatePath = Path.Combine(webhostEnvironment.WebRootPath, "templates", "CVHtmlTemplate.html");
    var templateHtml = File.ReadAllText(templatePath);
    return new PdfExportService(templateHtml);
});

builder.Services.AddSingleton<ILocalFileStorageService>(serviceProvider =>
{
    var uploadsPath = Path.Combine(AppContext.BaseDirectory, "uploads", "files");
    return new LocalFileStorageService(uploadsPath);
});

// --- Tests & Interviews repositories ---
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IApplicantRepository, ApplicantRepository>();
builder.Services.AddScoped<ICollaboratorsRepo, CollaboratorsRepo>();
builder.Services.AddScoped<ICompanyRepo, CompanyRepo>();
builder.Services.AddScoped<IEventsRepo, EventsRepo>();
builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<IJobsRepository, JobsRepository>();
builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ISlotRepository, SlotRepository>();
builder.Services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
builder.Services.AddScoped<ITestRepository, TestRepository>();

// --- Tests & Interviews services ---
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IApplicantService, ApplicantService>();
builder.Services.AddScoped<IAttemptValidationService, AttemptValidationService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICollaboratorsService, CollaboratorsService>();
builder.Services.AddScoped<ICompanyStatsService, CompanyStatsService>();
builder.Services.AddScoped<IDataProcessingService, DataProcessingService>();
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGradingService, GradingService>();
builder.Services.AddScoped<IInterviewSessionService, InterviewSessionService>();
builder.Services.AddScoped<IJobsService, JobsService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProfileCompletionCalculator, ProfileCompletionCalculator>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<ITestAttemptService, TestAttemptService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<ITimerService, TimerService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Run migrations for both contexts at startup
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<PussyCatsDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jobs API v1");
        c.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseCors("AllowWebProject");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
