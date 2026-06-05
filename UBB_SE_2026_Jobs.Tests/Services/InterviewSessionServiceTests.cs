using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Tests.Fakes;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class InterviewSessionServiceTests
{
    private readonly FakeInterviewSessionRepository interviewSessionRepository = new();
    private readonly InterviewSessionService interviewSessionService;

    public InterviewSessionServiceTests()
    {
        interviewSessionService = new InterviewSessionService(interviewSessionRepository);
    }


    [Fact]
    public async Task GetScheduledSessionsAsync_NoSessionsExist_ReturnsEmptyList()
    {
        var result = await interviewSessionService.GetScheduledSessionsAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetScheduledSessionsAsync_OnlyScheduledSessions_ReturnsAllOfThem()
    {
        const int expectedNumberOfSessions = 2;
        interviewSessionRepository.Seed(
            new InterviewSession { Id = 1, Status = InterviewStatus.Scheduled.ToString() },
            new InterviewSession { Id = 2, Status = InterviewStatus.Scheduled.ToString() });

        var result = await interviewSessionService.GetScheduledSessionsAsync();

        Assert.Equal(expectedNumberOfSessions, result.Count);
    }

    [Fact]
    public async Task GetScheduledSessionsAsync_MixedStatuses_ReturnsOnlyScheduled()
    {
        int scheduledInterviewId = 1;
        interviewSessionRepository.Seed(
            new InterviewSession { Id = scheduledInterviewId, Status = InterviewStatus.Scheduled.ToString() },
            new InterviewSession { Id = 2, Status = InterviewStatus.Completed.ToString() },
            new InterviewSession { Id = 3, Status = InterviewStatus.InProgress.ToString() });

        var result = await interviewSessionService.GetScheduledSessionsAsync();

        Assert.Single(result);
        Assert.Equal(scheduledInterviewId, result[0].Id);
    }


    [Fact]
    public async Task GetScheduledSessionsForCandidateAsync_CandidateHasSessions_ReturnsOnlyTheirSessions()
    {
        const int userId = 10;
        const int secondUserId = 99;
        const int expectedNumberOfSessions = 2;
        interviewSessionRepository.Seed(
            new InterviewSession { Id = 1, ExternalUserId = userId, Status = InterviewStatus.Scheduled.ToString() },
            new InterviewSession { Id = 2, ExternalUserId = secondUserId, Status = InterviewStatus.Scheduled.ToString() },
            new InterviewSession { Id = 3, ExternalUserId = userId, Status = InterviewStatus.Scheduled.ToString() });

        var result = await interviewSessionService.GetScheduledSessionsForCandidateAsync(10);

        Assert.Equal(expectedNumberOfSessions, result.Count);
        Assert.All(result, s => Assert.Equal(userId, s.ExternalUserId));
    }

    [Fact]
    public async Task GetScheduledSessionsForCandidateAsync_CandidateHasNoSessions_ReturnsEmpty()
    {
        interviewSessionRepository.Seed(
            new InterviewSession { Id = 1, ExternalUserId = 99, Status = InterviewStatus.Scheduled.ToString() });

        var result = await interviewSessionService.GetScheduledSessionsForCandidateAsync(10);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetScheduledSessionsForCandidateAsync_CandidateSessionsNotScheduled_ReturnsEmpty()
    {
        interviewSessionRepository.Seed(
            new InterviewSession { Id = 1, ExternalUserId = 10, Status = InterviewStatus.Completed.ToString() });

        var result = await interviewSessionService.GetScheduledSessionsForCandidateAsync(10);

        Assert.Empty(result);
    }


    [Fact]
    public async Task GetInterviewsByStatusAsync_StatusExists_ReturnsMatchingSessions()
    {
        int completedInterviewSessionId = 1;
        interviewSessionRepository.Seed(
            new InterviewSession { Id = completedInterviewSessionId, Status = InterviewStatus.Completed.ToString() },
            new InterviewSession { Id = 2, Status = InterviewStatus.Scheduled.ToString() });

        var result = await interviewSessionService.GetInterviewsByStatusAsync(InterviewStatus.Completed.ToString());

        Assert.Single(result);
        Assert.Equal(completedInterviewSessionId, result[0].Id);
    }

    [Fact]
    public async Task GetInterviewsByStatusAsync_NoMatchingStatus_ReturnsEmpty()
    {
        interviewSessionRepository.Seed(
            new InterviewSession { Id = 1, Status = InterviewStatus.Scheduled.ToString() });

        var result = await interviewSessionService.GetInterviewsByStatusAsync(InterviewStatus.Completed.ToString());

        Assert.Empty(result);
    }


    [Fact]
    public async Task GetInterviewByIdAsync_SessionExists_ReturnsSession()
    {
        const int sessionId = 5;
        interviewSessionRepository.Seed(new InterviewSession { Id = sessionId, Status = InterviewStatus.Scheduled.ToString() });

        var result = await interviewSessionService.GetInterviewByIdAsync(sessionId);

        Assert.NotNull(result);
        Assert.Equal(sessionId, result.Id);
    }

    [Fact]
    public async Task GetInterviewByIdAsync_SessionMissing_ThrowsKeyNotFoundException()
    {
        Func<Task> act = () => interviewSessionService.GetInterviewByIdAsync(404);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddInterviewAsync_ValidSession_PersistsAndReturnsSession()
    {
        int externalUserId = 7;
        var session = new InterviewSession { ExternalUserId = externalUserId, Status = InterviewStatus.Scheduled.ToString() };

        var result = await interviewSessionService.AddInterviewAsync(session);

        Assert.NotNull(result);
        Assert.Equal(externalUserId, result.ExternalUserId);
        var allInteviewSessions = await interviewSessionService.GetScheduledSessionsAsync();
        Assert.Single(allInteviewSessions);
    }


    [Fact]
    public async Task UpdateInterviewAsync_SessionExists_UpdatesFieldsAndPreservesVideo()
    {
        const int sessionId = 3, externalUserId = 2;
        string videoFilePath = "Videos/existing.mp4";
        interviewSessionRepository.Seed(new InterviewSession
        {
            Id = sessionId,
            ExternalUserId = externalUserId,
            Status = InterviewStatus.Scheduled.ToString(),
            Video = videoFilePath
        });
        var updated = new InterviewSession { ExternalUserId = externalUserId, Status = InterviewStatus.InProgress.ToString() };

        var result = await interviewSessionService.UpdateInterviewAsync(sessionId, updated);

        Assert.Equal(sessionId, result.Id);
        Assert.Equal(externalUserId, result.ExternalUserId);
        Assert.Equal(InterviewStatus.InProgress.ToString(), result.Status);
        Assert.Equal(videoFilePath, result.Video);
    }

    [Fact]
    public async Task UpdateInterviewAsync_SessionMissing_ThrowsKeyNotFoundException()
    {
        const int nonExistentInterviewSessionId = 999;
        var updated = new InterviewSession { Status = InterviewStatus.Completed.ToString() };

        Func<Task> act = () => interviewSessionService.UpdateInterviewAsync(nonExistentInterviewSessionId, updated);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteInterviewAsync_SessionExists_RemovesItAndReturnsTrue()
    {
        const int sessionId = 8;
        interviewSessionRepository.Seed(new InterviewSession { Id = sessionId, Status = InterviewStatus.Scheduled.ToString() });

        var result = await interviewSessionService.DeleteInterviewAsync(sessionId);

        Assert.True(result);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => interviewSessionService.GetInterviewByIdAsync(sessionId));
    }

    [Fact]
    public async Task DeleteInterviewAsync_SessionMissing_ThrowsKeyNotFoundException()
    {
        Func<Task> act = () => interviewSessionService.DeleteInterviewAsync(404);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // UploadVideoAsync / GetVideoAsync
    // NOTE: These methods are tightly coupled to the real file system
    // (Directory.CreateDirectory, FileStream, File.ReadAllBytes) with no
    // injected abstraction.  Unit-testing them without touching disk requires
    // extracting an IFileSystem interface first — tracked as a refactor task.
    // Integration-level coverage for those two methods should live in a
    // separate integration test project that runs against a temp directory.
    // -------------------------------------------------------------------------
}