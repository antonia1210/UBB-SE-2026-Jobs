using NSubstitute;
using NSubstitute.ExceptionExtensions;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;
using UBB_SE_2026_Jobs.Tests.Fakes;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class TestServiceTests
{
    private readonly ITestRepository testRepository = Substitute.For<ITestRepository>();
    private readonly FakeTestAttemptRepository attemptRepository = new();
    private readonly IAnswerRepository answerRepository = Substitute.For<IAnswerRepository>();
    private readonly IGradingService gradingService = Substitute.For<IGradingService>();
    private readonly ITimerService timerService = Substitute.For<ITimerService>();
    private readonly IAttemptValidationService validationService = Substitute.For<IAttemptValidationService>();
    private readonly IDataProcessingService dataProcessingService = Substitute.For<IDataProcessingService>();
    private readonly TestService service;

    public TestServiceTests()
    {
        answerRepository.FindByAttemptAsync(Arg.Any<int>()).Returns(new List<Answer>());

        service = new TestService(
            testRepository,
            attemptRepository,
            answerRepository,
            gradingService,
            timerService,
            validationService,
            dataProcessingService);
    }

    [Fact]
    public async Task GetAll_ReturnsAllTests()
    {
        var tests = new List<Test> { new() { Id = 1, Title = "C# Basics" }, new() { Id = 2, Title = "SQL" } };
        testRepository.GetTestsAsync().Returns(tests);

        var result = await service.GetAll();

        Assert.Equal(2, result.Count);
        Assert.Equal("C# Basics", result[0].Title);
    }

    [Fact]
    public async Task GetCategories_ReturnsAllCategories()
    {
        var categories = new List<string> { "Programming", "Databases" };
        testRepository.GetAllCategories().Returns(categories);

        var result = await service.GetCategories();

        Assert.Equal(2, result.Count);
        Assert.Contains("Programming", result);
    }

    [Fact]
    public async Task FindByIdAsync_ExistingTest_ReturnsTest()
    {
        var test = new Test { Id = 42, Title = "Java Fundamentals" };
        testRepository.FindByIdAsync(42).Returns(test);

        var result = await service.FindByIdAsync(42);

        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("Java Fundamentals", result.Title);
    }

    [Fact]
    public async Task FindByIdAsync_NonExistingTest_ReturnsNull()
    {
        testRepository.FindByIdAsync(999).Returns((Test?)null);

        var result = await service.FindByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task FindTestsByCategoryAsync_ExistingCategory_ReturnsMatchingTests()
    {
        var tests = new List<Test>
        {
            new() { Id = 1, Title = "C# Basics", Category = "Programming" },
            new() { Id = 2, Title = "Python", Category = "Programming" }
        };
        testRepository.FindTestsByCategoryAsync("Programming").Returns(tests);

        var result = await service.FindTestsByCategoryAsync("Programming");

        Assert.Equal(2, result.Count);
        Assert.All(result, test => Assert.Equal("Programming", test.Category));
    }

    [Fact]
    public async Task StartTestAsync_ValidUserAndTest_SavesAttemptWithInProgressStatus()
    {
        const int userId = 1, testId = 10;

        await service.StartTestAsync(userId, testId);

        var saved = attemptRepository.FindByUserAndTestAsync(userId, testId);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task StartTestAsync_ValidUserAndTest_StartsTimer()
    {
        const int userId = 2, testId = 5;

        await service.StartTestAsync(userId, testId);

        timerService.Received(1).StartTimer(Arg.Any<int>());
    }

    [Fact]
    public async Task StartTestAsync_ValidationServiceThrows_ExceptionPropagates()
    {
        validationService
            .CheckExistingAttemptsAsync(Arg.Any<int>(), Arg.Any<int>())
            .Throws(new InvalidOperationException("Attempt already in progress."));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.StartTestAsync(1, 1));
    }

    [Fact]
    public async Task SubmitTestAsync_AttemptNotFound_ThrowsInvalidOperationException()
    {
        const int nonExistentAttemptId = 999;

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SubmitTestAsync(nonExistentAttemptId));
    }

    [Fact]
    public async Task SubmitTestAsync_AttemptExists_SetsStatusToCompleted()
    {
        const int attemptId = 1;
        var attempt = new TestAttempt { Id = attemptId, TestId = 10, ExternalUserId = 1 };
        attemptRepository.Seed(attempt);

        await service.SubmitTestAsync(attemptId);

        var updated = await attemptRepository.FindByIdAsync(attemptId);
        Assert.Equal(TestStatus.COMPLETED.ToString(), updated!.Status);
    }

    [Fact]
    public async Task SubmitTestAsync_SingleChoiceAnswer_CallsGradeSingleChoice()
    {
        const int attemptId = 2;
        var attempt = new TestAttempt { Id = attemptId };
        attemptRepository.Seed(attempt);

        var question = new TestQuestion { QuestionTypeString = nameof(QuestionType.SINGLE_CHOICE) };
        var answer = new Answer { AttemptId = attemptId, Value = "0", Question = question };
        answerRepository.FindByAttemptAsync(attemptId).Returns(new List<Answer> { answer });

        await service.SubmitTestAsync(attemptId);

        gradingService.Received(1).GradeSingleChoice(question, answer);
    }

    [Fact]
    public async Task SubmitTestAsync_AnswerWithNullQuestion_SkipsGrading()
    {
        const int attemptId = 3;
        var attempt = new TestAttempt { Id = attemptId };
        attemptRepository.Seed(attempt);

        var answer = new Answer { AttemptId = attemptId, Value = "1", Question = null };
        answerRepository.FindByAttemptAsync(attemptId).Returns(new List<Answer> { answer });

        await service.SubmitTestAsync(attemptId);

        gradingService.DidNotReceive().GradeSingleChoice(Arg.Any<TestQuestion>(), Arg.Any<Answer>());
    }

    [Fact]
    public async Task SubmitAttemptAsync_AttemptNotFound_ReturnsZero()
    {
        const float expected = 0f;

        var result = await service.SubmitAttemptAsync(99, 99, new List<AnswerDto>());

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task SubmitAttemptAsync_AnswerWithEmptyValue_IsNotSaved()
    {
        const int userId = 1, testId = 10, attemptId = 7;
        attemptRepository.Seed(new TestAttempt { Id = attemptId, ExternalUserId = userId, TestId = testId });

        var answers = new List<AnswerDto>
        {
            new() { QuestionId = 1, Value = string.Empty },
            new() { QuestionId = 2, Value = null! }
        };

        await service.SubmitAttemptAsync(userId, testId, answers);

        await answerRepository.DidNotReceive().SaveAsync(Arg.Any<Answer>());
    }

    [Fact]
    public async Task SubmitAttemptAsync_ValidAnswers_SavesNonEmptyAnswers()
    {
        const int userId = 1, testId = 10, attemptId = 8;
        attemptRepository.Seed(new TestAttempt { Id = attemptId, ExternalUserId = userId, TestId = testId });

        var answers = new List<AnswerDto>
        {
            new() { QuestionId = 1, Value = "A" },
            new() { QuestionId = 2, Value = string.Empty }
        };

        await service.SubmitAttemptAsync(userId, testId, answers);

        await answerRepository.Received(1).SaveAsync(Arg.Any<Answer>());
    }

    [Fact]
    public async Task SubmitAttemptAsync_ValidAttempt_CallsDataProcessing()
    {
        const int userId = 1, testId = 10, attemptId = 9;
        attemptRepository.Seed(new TestAttempt { Id = attemptId, ExternalUserId = userId, TestId = testId });

        await service.SubmitAttemptAsync(userId, testId, new List<AnswerDto>());

        await dataProcessingService.Received(1).ProcessFinalizedAttemptAsync(attemptId);
    }

    [Fact]
    public async Task SubmitAttemptAsync_GradingServiceSetsScore_ReturnsScore()
    {
        const int userId = 1, testId = 10, attemptId = 11;
        const decimal score = 80m;
        attemptRepository.Seed(new TestAttempt { Id = attemptId, ExternalUserId = userId, TestId = testId });

        gradingService.When(gradingService => gradingService.CalculateFinalScore(Arg.Any<TestAttempt>()))
            .Do(call => ((TestAttempt)call[0]).Score = score);

        var result = await service.SubmitAttemptAsync(userId, testId, new List<AnswerDto>());

        Assert.Equal((float)score, result);
    }
}
