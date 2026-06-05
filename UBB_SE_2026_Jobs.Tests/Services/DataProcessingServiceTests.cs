using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Repositories.Users;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class DataProcessingServiceTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly ITestAttemptRepository testAttemptRepository = Substitute.For<ITestAttemptRepository>();
    private readonly ITestRepository testRepository = Substitute.For<ITestRepository>();
    private readonly DataProcessingService dataProcessingService;

    public DataProcessingServiceTests()
    {
        dataProcessingService = new DataProcessingService(userRepository, testAttemptRepository, testRepository);
    }

    // Helpers

    private void SetupValidDependencies(TestAttempt attempt)
    {
        userRepository.GetByIdAsync(attempt.ExternalUserId!.Value)
            .Returns(new User { Id = attempt.ExternalUserId.Value });

        testRepository.FindByIdAsync(attempt.TestId)
            .Returns(new Test { Id = attempt.TestId });
    }

    private static TestAttempt ValidAttempt() => new()
    {
        Id = 1,
        ExternalUserId = 10,
        TestId = 20,
        Status = "COMPLETED",
        Score = 80m,
        CompletedAt = DateTime.UtcNow,
        Answers = new List<Answer>
        {
            new() { Value = "CORRECT:50", Question = new TestQuestion { QuestionScore = 50f } },
            new() { Value = "CORRECT:50", Question = new TestQuestion { QuestionScore = 50f } },
        },
    };

    private static void AssertRejected(TestAttempt attempt, string expectedReason)
    {
        Assert.False(attempt.IsValidated);
        Assert.Null(attempt.PercentageScore);
        Assert.Equal(expectedReason, attempt.RejectionReason);
        Assert.NotNull(attempt.RejectedAt);
    }

    // ProcessFinalizedAttemptAsync — attempt not found

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_AttemptNotFound_ReturnsFalseWithoutUpdating()
    {
        testAttemptRepository.FindByIdAsync(999).Returns((TestAttempt?)null);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attemptId: 999);

        Assert.False(result);
        await testAttemptRepository.DidNotReceive().UpdateAsync(Arg.Any<TestAttempt>());
    }

    // ProcessFinalizedAttemptAsync — validation failures

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_NullExternalUserId_RejectsWithUserDoesNotExistReason()
    {
        var attempt = ValidAttempt();
        attempt.ExternalUserId = null;
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.False(result);
        AssertRejected(attempt, expectedReason: "User does not exist.");
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_UserNotFoundInRepository_RejectsWithUserDoesNotExistReason()
    {
        var attempt = ValidAttempt();
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        userRepository.GetByIdAsync(attempt.ExternalUserId!.Value).Returns((User?)null);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.False(result);
        AssertRejected(attempt, expectedReason: "User does not exist.");
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_TestNotFound_RejectsWithTestDoesNotExistReason()
    {
        var attempt = ValidAttempt();
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        userRepository.GetByIdAsync(attempt.ExternalUserId!.Value)
            .Returns(new User { Id = attempt.ExternalUserId.Value });
        testRepository.FindByIdAsync(attempt.TestId).Returns((Test?)null);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.False(result);
        AssertRejected(attempt, expectedReason: "Test does not exist.");
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_NullCompletedAt_RejectsWithIncompleteAttemptReason()
    {
        var attempt = ValidAttempt();
        attempt.CompletedAt = null;
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.False(result);
        AssertRejected(attempt, expectedReason: "Attempt is incomplete. Missing completion time.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ProcessFinalizedAttemptAsync_NullOrWhitespaceStatus_RejectsWithStatusMissingReason(string? status)
    {
        var attempt = ValidAttempt();
        attempt.Status = status;
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.False(result);
        AssertRejected(attempt, expectedReason: "Attempt status is missing.");
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("IN_PROGRESS")]
    [InlineData("CANCELLED")]
    public async Task ProcessFinalizedAttemptAsync_StatusNotCompleted_RejectsWithStatusNotCompletedReason(string status)
    {
        var attempt = ValidAttempt();
        attempt.Status = status;
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.False(result);
        AssertRejected(attempt, expectedReason: "Attempt is not eligible for leaderboard because status is not COMPLETED.");
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_NegativeScore_RejectsWithInvalidScoreReason()
    {
        var attempt = ValidAttempt();
        attempt.Score = -1m;
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.False(result);
        AssertRejected(attempt, expectedReason: "Attempt score is invalid.");
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_ValidationFails_UpdatesAttemptExactlyOnce()
    {
        var attempt = ValidAttempt();
        attempt.ExternalUserId = null;
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);

        await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        await testAttemptRepository.Received(1).UpdateAsync(attempt);
    }

    // ProcessFinalizedAttemptAsync — happy path

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_AllValidationsPass_ReturnsTrue()
    {
        var attempt = ValidAttempt();
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_AllValidationsPass_MarksAttemptAsValidated()
    {
        var attempt = ValidAttempt();
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.True(attempt.IsValidated);
        Assert.Null(attempt.RejectionReason);
        Assert.Null(attempt.RejectedAt);
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_AllValidationsPass_UpdatesAttemptExactlyOnce()
    {
        var attempt = ValidAttempt();
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        await testAttemptRepository.Received(1).UpdateAsync(attempt);
    }

    // ProcessFinalizedAttemptAsync — percentage score calculation

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_ScoreEqualsMaxPossible_SetsPercentageScoreToOneHundred()
    {
        var attempt = ValidAttempt();
        attempt.Score = 100m;
        attempt.Answers = new List<Answer>
        {
            new() { Question = new TestQuestion { QuestionScore = 100f } },
        };
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.Equal(100m, attempt.PercentageScore);
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_ScoreIsHalfOfMaxPossible_SetsPercentageScoreToFifty()
    {
        var attempt = ValidAttempt();
        attempt.Score = 50m;
        attempt.Answers = new List<Answer>
        {
            new() { Question = new TestQuestion { QuestionScore = 100f } },
        };
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.Equal(50m, attempt.PercentageScore);
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_ScoreIsZero_SetsPercentageScoreToZero()
    {
        var attempt = ValidAttempt();
        attempt.Score = 0m;
        attempt.Answers = new List<Answer>
        {
            new() { Question = new TestQuestion { QuestionScore = 100f } },
        };
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.Equal(0m, attempt.PercentageScore);
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_MaxPossibleScoreIsZero_SetsPercentageScoreToZero()
    {
        var attempt = ValidAttempt();
        attempt.Score = 0m;
        attempt.Answers = new List<Answer>
        {
            new() { Question = new TestQuestion { QuestionScore = 0f } },
        };
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.Equal(0m, attempt.PercentageScore);
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_AnswerWithNullQuestion_CountsAsZeroWeightTowardMaxScore()
    {
        var attempt = ValidAttempt();
        attempt.Score = 50m;
        attempt.Answers = new List<Answer>
        {
            new() { Question = new TestQuestion { QuestionScore = 100f } },
            new() { Question = null },
        };
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        // max possible = 100 (null question contributes zero), score = 50 => 50%
        Assert.Equal(50m, attempt.PercentageScore);
    }

    // ProcessFinalizedAttemptAsync — status check is case-insensitive


    [Theory]
    [InlineData("completed")]
    [InlineData("COMPLETED")]
    [InlineData("Completed")]
    public async Task ProcessFinalizedAttemptAsync_StatusCompletedRegardlessOfCase_Validates(string status)
    {
        var attempt = ValidAttempt();
        attempt.Status = status;
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.True(result);
    }

    // ProcessFinalizedAttemptAsync — score boundary


    [Fact]
    public async Task ProcessFinalizedAttemptAsync_ScoreOfZero_IsValidAndNotRejected()
    {
        var attempt = ValidAttempt();
        attempt.Score = 0m;
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task ProcessFinalizedAttemptAsync_ScoreAboveOneHundred_IsValidAndNotRejected()
    {
        // Upper bound was removed in the refactor — scores above 100 are now accepted.
        var attempt = ValidAttempt();
        attempt.Score = 101m;
        attempt.Answers = new List<Answer>
        {
            new() { Question = new TestQuestion { QuestionScore = 200f } },
        };
        testAttemptRepository.FindByIdAsync(attempt.Id).Returns(attempt);
        SetupValidDependencies(attempt);

        var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

        Assert.True(result);
    }
}
