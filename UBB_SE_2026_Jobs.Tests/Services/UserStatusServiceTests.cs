using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;
using UBB_SE_2026_Jobs.Library.Services.PussyCatsCompanyService;
using UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;
using UBB_SE_2026_Jobs.Tests.Fakes;
using UBB_SE_2026_Jobs.Tests.Helpers;
using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Library.Services.JobSkills;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class UserStatusServiceTests
{
    private const int UserId = 1;
    private const int MissingJobId = 999;
    private const int CompanyId = 5;
    private const int MissingCompanyId = 99;
    private const int JobId = 10;
    private const int MatchId = 1;
    private const int SkillId = 100;
    private const int SkillScore = 80;
    private const int RequiredSkillLevel = 80;
    private const string KnownCompanyName = "Acme";
    private const string UnknownCompanyName = "Unknown Company";

    private readonly FakeMatchRepository matchRepository = new();
    private readonly FakeJobRepository jobRepository = new();
    private readonly FakeCompanyRepository companyRepository = new();
    private readonly FakeUserSkillRepository userSkillRepository = new();
    private readonly FakeJobSkillRepository jobSkillRepository = new();
    private readonly FakeUserRepository userRepository = new();
    private readonly UserStatusService service;

    public UserStatusServiceTests()
    {
        service = new UserStatusService(
            matchRepository,
            new PussyCatsJobService(jobRepository),
            new PussyCatsCompanyService(companyRepository),
            new UserSkillService(userSkillRepository),
            new JobSkillService(jobSkillRepository),
            userRepository,
            new RecommendationAlgorithm());
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_UserHasNoMatches_ReturnsEmptyList()
    {
        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_MatchHasMissingJob_SkipsInvalidMatches()
    {
        matchRepository.Seed(
            new MatchBuilder()
                .WithId(MatchId)
                .AppliedFor(UserId, MissingJobId)
                .Build());

        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_ValidMatchExists_ReturnsSingleApplication()
    {
        SeedValidApplication();

        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_ValidMatchExists_ReturnsCorrectCompanyName()
    {
        SeedValidApplication();

        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.Equal(KnownCompanyName, result[0].CompanyName);
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_ValidMatchExists_ReturnsCompatibilityScoreInValidRange()
    {
        SeedValidApplication();

        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.InRange(result[0].CompatibilityScore, 0, 100);
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_ValidMatchExists_ReturnsCorrectMatchId()
    {
        SeedValidApplication();

        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.Equal(MatchId, result[0].MatchId);
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_ValidMatchExists_ReturnsCorrectJobId()
    {
        SeedValidApplication();

        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.Equal(JobId, result[0].JobId);
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_UserNotFound_ReturnsZeroCompatibilityScore()
    {
        companyRepository.Seed(new CompanyBuilder().WithId(CompanyId).WithName(KnownCompanyName).Build());
        jobRepository.Seed(new JobBuilder().WithId(JobId).WithCompanyId(CompanyId).Build());
        matchRepository.Seed(new MatchBuilder().WithId(MatchId).AppliedFor(UserId, JobId).WithStatus(MatchStatus.Applied).Build());

        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.Equal(0, result[0].CompatibilityScore);
    }

    [Fact]
    public async Task GetApplicationsForUserAsync_CompanyIsMissing_FallsBackToUnknownCompanyName()
    {
        jobRepository.Seed(
            new JobBuilder()
                .WithId(JobId)
                .WithCompanyId(MissingCompanyId)
                .Build());

        matchRepository.Seed(
            new MatchBuilder()
                .WithId(MatchId)
                .AppliedFor(UserId, JobId)
                .Build());

        var result = await service.GetApplicationsForUserAsync(UserId);

        Assert.Equal(UnknownCompanyName, result[0].CompanyName);
    }

    private void SeedValidApplication()
    {
        userRepository.Seed(new User { UserId = UserId });

        companyRepository.Seed(
            new CompanyBuilder()
                .WithId(CompanyId)
                .WithName(KnownCompanyName)
                .Build());

        jobRepository.Seed(
            new JobBuilder()
                .WithId(JobId)
                .WithCompanyId(CompanyId)
                .Build());

        matchRepository.Seed(
            new MatchBuilder()
                .WithId(MatchId)
                .AppliedFor(UserId, JobId)
                .WithStatus(MatchStatus.Applied)
                .Build());

        userSkillRepository.Seed(
            new UserSkill
            {
                User = new User { UserId = UserId },
                Skill = new Skill { SkillId = SkillId },
                Score = SkillScore
            });

        jobSkillRepository.Seed(
            new JobSkill
            {
                Job = new Job { JobId = JobId },
                Skill = new Skill { SkillId = SkillId },
                RequiredLevel = RequiredSkillLevel
            });
    }
}
