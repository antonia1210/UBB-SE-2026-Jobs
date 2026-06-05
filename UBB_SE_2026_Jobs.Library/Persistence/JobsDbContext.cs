using Microsoft.EntityFrameworkCore;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Persistence;

public class JobsDbContext : DbContext
{
    public JobsDbContext(DbContextOptions<JobsDbContext> options)
        : base(options)
    {
    }

    // PussyCats Entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<PersonalityTestResult> PersonalityTestResults => Set<PersonalityTestResult>();
    public DbSet<SkillGroup> SkillGroups => Set<SkillGroup>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<JobSkill> JobSkills => Set<JobSkill>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<WorkExperience> WorkExperiences => Set<WorkExperience>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ExtraCurricularActivity> ExtraCurricularActivities => Set<ExtraCurricularActivity>();

    // TestsAndInterviews Entities
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Collaborator> Collaborators => Set<Collaborator>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
    public DbSet<LeaderboardEntry> LeaderboardEntries => Set<LeaderboardEntry>();
    public DbSet<Recruiter> Recruiters => Set<Recruiter>();
    public DbSet<Slot> Slots => Set<Slot>();
    public DbSet<InterviewSession> InterviewSessions => Set<InterviewSession>();
    public DbSet<TestQuestion> Questions => Set<TestQuestion>();
    public DbSet<PersonalityQuestion> PersonalityQuestions => Set<PersonalityQuestion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobsDbContext).Assembly);
    }
}
