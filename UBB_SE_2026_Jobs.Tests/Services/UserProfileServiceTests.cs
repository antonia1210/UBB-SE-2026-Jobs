using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.UserProfileService;
using UBB_SE_2026_Jobs.Tests.Fakes;
using UBB_SE_2026_Jobs.Tests.Helpers;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class UserProfileServiceTests
{
    private const int MissingUserId = 99;
    private const int ExistingUserId = 1;
    private const int NewUserId = 0;
    private const int OneMinute = 1;

    private const string OldEmail = "old@x.com";
    private const string NewEmail = "new@x.com";
    private const string ProfilePicturePath = "pic.png";
    private const string OldProfilePicturePath = "old.png";
    private const string UniversityName = "Cambridge";
    private const string FirstName = "Ada";
    private const string LastName = "Lovelace";
    private const string PrimarySkillName = "C#";
    private const string SecondarySkillName = "SQL";
    private const int SkillIdOne = 1;
    private const int SkillIdTwo = 2;

    private readonly FakeUserRepository userRepository = new();
    private readonly FakeTestAttemptRepository testAttemptRepository = new();
    private readonly UserProfileService service;

    public UserProfileServiceTests()
    {
        service = new UserProfileService(userRepository, testAttemptRepository);
    }

    [Fact]
    public async Task IsProfileAvailableAsync_UserIsMissing_ThrowsException()
    {
        Func<Task> act = () => service.IsProfileAvailableAsync(MissingUserId);
        var exception = await Assert.ThrowsAsync<Exception>(act);
        Assert.Contains("No profile found", exception.Message);
    }

    [Fact]
    public async Task IsProfileAvailableAsync_ProfileExists_ReturnsActiveAccountFlag()
    {
        userRepository.Seed(new UserBuilder().WithId(ExistingUserId).WithActiveAccount(false).Build());
        Assert.False(await service.IsProfileAvailableAsync(ExistingUserId));
    }

    [Fact]
    public async Task UpdateAccountStatusAsync_StatusChanged_PersistsStatusAndTouchesLastUpdated()
    {
        userRepository.Seed(new UserBuilder().WithId(ExistingUserId).WithActiveAccount(true).Build());
        await service.UpdateAccountStatusAsync(ExistingUserId, false);
        var user = await userRepository.GetByIdAsync(ExistingUserId);
        Assert.False(user!.ActiveAccount);
        Assert.True(user.LastUpdated > DateTime.UtcNow.AddMinutes(-OneMinute));
    }

    [Fact]
    public async Task UpdateProfilePicturePathAsync_PathProvided_WritesPathToUser()
    {
        userRepository.Seed(new UserBuilder().WithId(ExistingUserId).Build());
        await service.UpdateProfilePicturePathAsync(ExistingUserId, ProfilePicturePath);
        Assert.Equal(ProfilePicturePath, (await userRepository.GetByIdAsync(ExistingUserId))!.ProfilePicturePath);
    }

    [Fact]
    public async Task UpdateProfilePicturePathAsync_PathIsNull_HandlesAsEmptyString()
    {
        userRepository.Seed(new UserBuilder().WithId(ExistingUserId).Build());
        await service.UpdateProfilePicturePathAsync(ExistingUserId, null!);
        Assert.Empty((await userRepository.GetByIdAsync(ExistingUserId))!.ProfilePicturePath);
    }

    [Fact]
    public async Task RemoveProfilePicturePathAsync_UserHasPicture_ClearsPath()
    {
        var user = new UserBuilder().WithId(ExistingUserId).Build();
        user.ProfilePicturePath = OldProfilePicturePath;
        userRepository.Seed(user);
        await service.RemoveProfilePicturePathAsync(ExistingUserId);
        Assert.Empty((await userRepository.GetByIdAsync(ExistingUserId))!.ProfilePicturePath);
    }

    [Fact]
    public async Task SaveAsync_UserIsMissing_AddsNewUser()
    {
        var user = new UserBuilder().WithId(NewUserId).Build();
        await service.SaveAsync(NewUserId, user);
        Assert.Equal(1, (await userRepository.GetAllAsync()).Count());
    }

    [Fact]
    public async Task SaveAsync_UserExists_UpdatesExistingUser()
    {
        userRepository.Seed(new UserBuilder().WithId(ExistingUserId).WithEmail(OldEmail).Build());
        var updated = new UserBuilder().WithId(ExistingUserId).WithEmail(NewEmail).Build();
        await service.SaveAsync(ExistingUserId, updated);
        Assert.Equal(NewEmail, (await userRepository.GetByIdAsync(ExistingUserId))!.Email);
    }

    [Fact]
    public void GenerateParsedCvText_UserIsNull_ReturnsEmptyString()
    {
        Assert.Empty(UBB_SE_2026_Jobs.Library.Services.Helpers.GenerateParsedCvText(null!));
    }

    [Fact]
    public void GenerateParsedCvText_ValidUserProvided_CombinesNameUniversityAndSkillNames()
    {
        var user = new UserBuilder().WithId(ExistingUserId).WithName(FirstName, LastName).Build();
        user.University = UniversityName;
        user.Skills = new List<UserSkill>
        {
            new() { Skill = new Skill { SkillId = SkillIdOne, Name = PrimarySkillName } },
            new() { Skill = new Skill { SkillId = SkillIdTwo, Name = SecondarySkillName } },
        };
        var text = UBB_SE_2026_Jobs.Library.Services.Helpers.GenerateParsedCvText(user);
        Assert.Contains($"{FirstName} {LastName}", text);
        Assert.Contains(UniversityName, text);
        Assert.Contains($"{PrimarySkillName}, {SecondarySkillName}", text);
    }

    [Fact]
    public async Task RecalculateLevelAsync_UserIsNull_ReturnsZero()
    {
        Assert.Equal(0, await service.RecalculateLevelAsync(null!));
    }

    [Fact]
    public async Task RecalculateLevelAsync_UserHasCompletedAttempts_SumsXpAndSetsLevel()
    {
        var user = new UserBuilder().WithId(ExistingUserId).Build();
        userRepository.Seed(user);

        testAttemptRepository.Seed(
            new TestAttempt { Id = 1, ExternalUserId = ExistingUserId, Status = "COMPLETED", IsValidated = true, CompletedAt = DateTime.UtcNow, PercentageScore = 95 },
            new TestAttempt { Id = 2, ExternalUserId = ExistingUserId, Status = "COMPLETED", IsValidated = true, CompletedAt = DateTime.UtcNow, PercentageScore = 75 },
            new TestAttempt { Id = 3, ExternalUserId = ExistingUserId, Status = "COMPLETED", IsValidated = true, CompletedAt = DateTime.UtcNow, PercentageScore = 55 }
        );

        var totalXp = await service.RecalculateLevelAsync(user);

        var expected = SimpleModelOperations.GoldExperiencePoints
            + SimpleModelOperations.SilverExperiencePoints
            + SimpleModelOperations.BronzeExperiencePoints;
        Assert.Equal(expected, totalXp);
        Assert.Equal(expected, user.TotalExperiencePoints);
        Assert.Equal(SimpleModelOperations.CalculateLevelNumber(expected), user.CurrentLevel);
    }
}