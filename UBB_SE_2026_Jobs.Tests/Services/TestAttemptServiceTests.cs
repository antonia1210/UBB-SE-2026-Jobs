using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class TestAttemptServiceTests
{
    private readonly ITestAttemptRepository testAttemptRepository = Substitute.For<ITestAttemptRepository>();
    private readonly TestAttemptService testAttemptService;

    public TestAttemptServiceTests()
    {
        testAttemptService = new TestAttemptService(testAttemptRepository);
    }

    [Fact]
    public async Task FindByIdAsync_AttemptExists_ReturnsIt()
    {
        var attempt = new TestAttempt { Id = 1 };
        testAttemptRepository.FindByIdAsync(1).Returns(attempt);

        var result = await testAttemptService.FindByIdAsync(1);

        Assert.Same(attempt, result);
    }

    [Fact]
    public async Task FindByIdAsync_AttemptMissing_ReturnsNull()
    {
        testAttemptRepository.FindByIdAsync(404).Returns((TestAttempt?)null);

        var result = await testAttemptService.FindByIdAsync(404);

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByUserAndTestAsync_AttemptExists_ReturnsIt()
    {
        var attempt = new TestAttempt { ExternalUserId = 5, TestId = 10 };
        testAttemptRepository.FindByUserAndTestAsync(5, 10).Returns(attempt);

        var result = await testAttemptService.FindByUserAndTestAsync(5, 10);

        Assert.Same(attempt, result);
    }

    [Fact]
    public async Task FindByUserAndTestAsync_AttemptMissing_ReturnsNull()
    {
        testAttemptRepository.FindByUserAndTestAsync(99, 10).Returns((TestAttempt?)null);

        var result = await testAttemptService.FindByUserAndTestAsync(99, 10);

        Assert.Null(result);
    }


    [Fact]
    public async Task SaveAsync_DelegatesToRepository()
    {
        var attempt = new TestAttempt { TestId = 3, ExternalUserId = 7 };

        await testAttemptService.SaveAsync(attempt);

        await testAttemptRepository.Received(1).SaveAsync(attempt);
    }


    [Fact]
    public async Task UpdateAsync_AttemptExists_ReturnsUpdated()
    {
        var attempt = new TestAttempt { Id = 2, PercentageScore = 85m };
        testAttemptRepository.UpdateAsync(attempt).Returns(attempt);

        var result = await testAttemptService.UpdateAsync(attempt);

        Assert.Same(attempt, result);
        await testAttemptRepository.Received(1).UpdateAsync(attempt);
    }

    [Fact]
    public async Task UpdateAsync_AttemptMissing_ReturnsNull()
    {
        var attempt = new TestAttempt { Id = 999 };
        testAttemptRepository.UpdateAsync(attempt).Returns((TestAttempt?)null);

        var result = await testAttemptService.UpdateAsync(attempt);

        Assert.Null(result);
    }

    [Fact]
    public async Task FindValidAttemptsByTestIdAsync_AttemptsExist_ReturnsThem()
    {
        var attempts = new List<TestAttempt>
        {
            new() { TestId = 5, ExternalUserId = 1, PercentageScore = 70m },
            new() { TestId = 5, ExternalUserId = 2, PercentageScore = 90m },
        };
        testAttemptRepository.FindValidAttemptsByTestIdAsync(5).Returns(attempts);

        var result = await testAttemptService.FindValidAttemptsByTestIdAsync(5);

        Assert.Same(attempts, result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task FindValidAttemptsByTestIdAsync_NoAttempts_ReturnsEmptyList()
    {
        testAttemptRepository.FindValidAttemptsByTestIdAsync(99).Returns(new List<TestAttempt>());

        var result = await testAttemptService.FindValidAttemptsByTestIdAsync(99);

        Assert.Empty(result);
    }
}
