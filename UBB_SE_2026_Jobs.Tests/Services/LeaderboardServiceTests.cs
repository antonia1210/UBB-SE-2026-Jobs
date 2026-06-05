using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class LeaderboardServiceTests
{
    private readonly ILeaderboardRepository leaderboardRepository = Substitute.For<ILeaderboardRepository>();
    private readonly ITestAttemptRepository testAttemptRepository = Substitute.For<ITestAttemptRepository>();
    private readonly LeaderboardService leaderboardService;

    public LeaderboardServiceTests()
    {
        leaderboardService = new LeaderboardService(leaderboardRepository, testAttemptRepository);
    }

    [Fact]
    public async Task FindByTestIdAsync_DelegatesToRepository()
    {
        var entries = new List<LeaderboardEntry> { new() { TestId = 1 } };
        leaderboardRepository.FindByTestIdAsync(1).Returns(entries);

        var result = await leaderboardService.FindByTestIdAsync(1);

        Assert.Same(entries, result);
        await leaderboardRepository.Received(1).FindByTestIdAsync(1);
    }


    [Fact]
    public async Task FindTopByTestIdAsync_DelegatesToRepository()
    {
        var entries = new List<LeaderboardEntry> { new() { TestId = 2, RankPosition = 1 } };
        leaderboardRepository.FindTopByTestIdAsync(2, 3).Returns(entries);

        var result = await leaderboardService.FindTopByTestIdAsync(2, 3);

        Assert.Same(entries, result);
        await leaderboardRepository.Received(1).FindTopByTestIdAsync(2, 3);
    }

    [Fact]
    public async Task FindUserEntryAsync_EntryExists_ReturnsIt()
    {
        var entry = new LeaderboardEntry { UserId = 5, TestId = 10 };
        leaderboardRepository.FindUserEntryAsync(5, 10).Returns(entry);

        var result = await leaderboardService.FindUserEntryAsync(5, 10);

        Assert.Same(entry, result);
    }

    [Fact]
    public async Task FindUserEntryAsync_EntryMissing_ReturnsNull()
    {
        leaderboardRepository.FindUserEntryAsync(99, 10).Returns((LeaderboardEntry?)null);

        var result = await leaderboardService.FindUserEntryAsync(99, 10);

        Assert.Null(result);
    }


    [Fact]
    public async Task DeleteByTestIdAsync_DelegatesToRepository()
    {
        await leaderboardService.DeleteByTestIdAsync(7);

        await leaderboardRepository.Received(1).DeleteByTestIdAsync(7);
    }


    [Fact]
    public async Task SaveRangeAsync_DelegatesToRepository()
    {
        var entries = new List<LeaderboardEntry> { new() { TestId = 3 } };

        await leaderboardService.SaveRangeAsync(entries);

        await leaderboardRepository.Received(1).SaveRangeAsync(entries);
    }

    [Fact]
    public async Task RecalculateAsync_WithAttempts_DeletesThenSavesRankedEntries()
    {
        const int testId = 1;
        var attempts = new List<TestAttempt>
        {
            new() { TestId = testId, ExternalUserId = 10, PercentageScore = 90m },
            new() { TestId = testId, ExternalUserId = 20, PercentageScore = 75m },
        };
        testAttemptRepository.FindValidAttemptsByTestIdAsync(testId).Returns(attempts);

        await leaderboardService.RecalculateAsync(testId);

        // Delete must happen before save — verify both were called
        await leaderboardRepository.Received(1).DeleteByTestIdAsync(testId);
        await leaderboardRepository.Received(1).SaveRangeAsync(Arg.Is<List<LeaderboardEntry>>(list =>
            list.Count == 2));
    }

    [Fact]
    public async Task RecalculateAsync_AssignsRanksInOrder()
    {
        const int testId = 2;
        var attempts = new List<TestAttempt>
        {
            new() { TestId = testId, ExternalUserId = 1, PercentageScore = 95m },
            new() { TestId = testId, ExternalUserId = 2, PercentageScore = 80m },
            new() { TestId = testId, ExternalUserId = 3, PercentageScore = 60m },
        };
        testAttemptRepository.FindValidAttemptsByTestIdAsync(testId).Returns(attempts);

        List<LeaderboardEntry>? saved = null;
        await leaderboardRepository.SaveRangeAsync(Arg.Do<List<LeaderboardEntry>>(list => saved = list));

        await leaderboardService.RecalculateAsync(testId);

        Assert.NotNull(saved);
        Assert.Equal(1, saved![0].RankPosition);
        Assert.Equal(2, saved[1].RankPosition);
        Assert.Equal(3, saved[2].RankPosition);
    }

    [Fact]
    public async Task RecalculateAsync_MapsAttemptFieldsCorrectly()
    {
        const int testId = 3;
        const int userId = 42;
        const decimal pct = 88.5m;
        var attempts = new List<TestAttempt>
        {
            new() { TestId = testId, ExternalUserId = userId, PercentageScore = pct },
        };
        testAttemptRepository.FindValidAttemptsByTestIdAsync(testId).Returns(attempts);

        List<LeaderboardEntry>? saved = null;
        await leaderboardRepository.SaveRangeAsync(Arg.Do<List<LeaderboardEntry>>(list => saved = list));

        var before = DateTime.UtcNow;
        await leaderboardService.RecalculateAsync(testId);
        var after = DateTime.UtcNow;

        Assert.NotNull(saved);
        var entry = saved![0];
        Assert.Equal(testId, entry.TestId);
        Assert.Equal(userId, entry.UserId);
        Assert.Equal(pct, entry.NormalizedScore);
        Assert.Equal(1, entry.RankPosition);
        Assert.Equal(1, entry.TieBreakPriority);
        Assert.InRange(entry.LastRecalculationAt, before, after);
    }

    [Fact]
    public async Task RecalculateAsync_WithNoAttempts_DeletesButDoesNotSave()
    {
        const int testId = 4;
        testAttemptRepository.FindValidAttemptsByTestIdAsync(testId).Returns(new List<TestAttempt>());

        await leaderboardService.RecalculateAsync(testId);

        await leaderboardRepository.Received(1).DeleteByTestIdAsync(testId);
        await leaderboardRepository.DidNotReceive().SaveRangeAsync(Arg.Any<List<LeaderboardEntry>>());
    }

    [Fact]
    public async Task RecalculateAsync_DeleteCalledBeforeSave()
    {
        const int testId = 5;
        var callOrder = new List<string>();

        testAttemptRepository.FindValidAttemptsByTestIdAsync(testId)
            .Returns(new List<TestAttempt>
            {
                new() { TestId = testId, ExternalUserId = 99, PercentageScore = 50m }
            });

        leaderboardRepository
            .When(r => r.DeleteByTestIdAsync(testId))
            .Do(_ => callOrder.Add("delete"));

        leaderboardRepository
            .When(r => r.SaveRangeAsync(Arg.Any<List<LeaderboardEntry>>()))
            .Do(_ => callOrder.Add("save"));

        await leaderboardService.RecalculateAsync(testId);

        Assert.Equal(new[] { "delete", "save" }, callOrder);
    }
}
