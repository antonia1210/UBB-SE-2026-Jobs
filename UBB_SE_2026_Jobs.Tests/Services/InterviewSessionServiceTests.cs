using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Tests.Fakes;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class InterviewSessionServiceTests
{
    private readonly FakeInterviewSessionRepository interviewSessionRepository = new();
    private readonly FakeApplicantRepository applicantRepository = new();
    private readonly FakeMatchRepository matchRepository = new();
    private readonly InterviewSessionService interviewSessionService;

    public InterviewSessionServiceTests()
    {
        interviewSessionService = new InterviewSessionService(interviewSessionRepository, applicantRepository, matchRepository);
    }


    private static InterviewSession SessionWithCandidate(int sessionId, int positionId, int candidateId) => new()
    {
        Id = sessionId,
        PositionId = positionId,
        ExternalUserId = candidateId,
        Status = InterviewStatus.Scheduled.ToString(),
    };

    private static Applicant ApplicantForSession(int applicantId, int jobId, int userId) => new()
    {
        ApplicantId = applicantId,
        JobId = jobId,
        UserId = userId,
        ApplicationStatus = "Pending",
    };


    [Fact]
    public async Task GetScheduledSessionsAsync_NoSessionsExist_ReturnsEmptyList()
    {
        var result = await interviewSessionService.GetScheduledSessionsAsync(null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetScheduledSessionsAsync_OnlyScheduledSessions_ReturnsAllOfThem()
    {
        const int expectedNumberOfSessions = 2;
        interviewSessionRepository.Seed(
            new InterviewSession { Id = 1, Status = InterviewStatus.Scheduled.ToString() },
            new InterviewSession { Id = 2, Status = InterviewStatus.Scheduled.ToString() });

        var result = await interviewSessionService.GetScheduledSessionsAsync(null);

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

        var result = await interviewSessionService.GetScheduledSessionsAsync(null);

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
        Assert.All(result, interviewSession => Assert.Equal(userId, interviewSession.ExternalUserId));
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

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddInterviewAsync_ValidSession_PersistsAndReturnsSession()
    {
        int externalUserId = 7;
        var session = new InterviewSession { ExternalUserId = externalUserId, Status = InterviewStatus.Scheduled.ToString() };

        var result = await interviewSessionService.AddInterviewAsync(session);

        Assert.NotNull(result);
        Assert.Equal(externalUserId, result.ExternalUserId);
        var allInteviewSessions = await interviewSessionService.GetScheduledSessionsAsync(null);
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

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
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

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(act);
        Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SetInterviewDecision_SessionNotFound_ThrowsKeyNotFoundException()
    {
        Func<Task> act = () => interviewSessionService.SetInterviewDecision(sessionId: 999, decision: "Accepted");

        await Assert.ThrowsAsync<KeyNotFoundException>(act);
    }

    [Fact]
    public async Task SetInterviewDecision_SessionHasNoCandidate_ThrowsInvalidDataException()
    {
        var sessionWithoutCandidate = new InterviewSession
        {
            Id = 1,
            PositionId = 10,
            ExternalUserId = null,
            Status = InterviewStatus.Scheduled.ToString(),
        };
        interviewSessionRepository.Seed(sessionWithoutCandidate);

        Func<Task> act = () => interviewSessionService.SetInterviewDecision(sessionId: 1, decision: "Accepted");

        var exception = await Assert.ThrowsAsync<InvalidDataException>(act);
        Assert.Contains("candidate", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SetInterviewDecision_ApplicantNotFound_CompletesWithoutException()
    {
        // Candidates who applied via the Match flow (not the legacy Applicant table) have no
        // Applicant row — the service should skip the applicant update and not throw.
        interviewSessionRepository.Seed(SessionWithCandidate(sessionId: 1, positionId: 10, candidateId: 5));

        await interviewSessionService.SetInterviewDecision(sessionId: 1, decision: "Accepted");

        var session = await interviewSessionRepository.GetInterviewSessionByIdAsync(1);
        Assert.Equal(InterviewStatus.Completed.ToString(), session!.Status);
    }

    [Fact]
    public async Task SetInterviewDecision_ValidSession_MarksSessionAsCompleted()
    {
        interviewSessionRepository.Seed(SessionWithCandidate(sessionId: 1, positionId: 10, candidateId: 5));
        applicantRepository.Seed(ApplicantForSession(applicantId: 1, jobId: 10, userId: 5));

        await interviewSessionService.SetInterviewDecision(sessionId: 1, decision: "Accepted");

        var updatedSession = await interviewSessionRepository.GetInterviewSessionByIdAsync(1);
        Assert.Equal(InterviewStatus.Completed.ToString(), updatedSession!.Status);
    }

    [Fact]
    public async Task SetInterviewDecision_ValidSession_SetsApplicantStatusToDecision()
    {
        interviewSessionRepository.Seed(SessionWithCandidate(sessionId: 1, positionId: 10, candidateId: 5));
        applicantRepository.Seed(ApplicantForSession(applicantId: 1, jobId: 10, userId: 5));

        await interviewSessionService.SetInterviewDecision(sessionId: 1, decision: "Accepted");

        var updatedApplicant = applicantRepository.GetPendingApplicantByJobAndUser(jobId: 10, userId: 5);
        Assert.Equal("Accepted", updatedApplicant!.ApplicationStatus);
    }

    [Fact]
    public async Task SetInterviewDecision_DecisionIsDeclined_SetsApplicantStatusToDeclined()
    {
        interviewSessionRepository.Seed(SessionWithCandidate(sessionId: 1, positionId: 10, candidateId: 5));
        applicantRepository.Seed(ApplicantForSession(applicantId: 1, jobId: 10, userId: 5));

        await interviewSessionService.SetInterviewDecision(sessionId: 1, decision: "Declined");

        var updatedApplicant = applicantRepository.GetPendingApplicantByJobAndUser(jobId: 10, userId: 5);
        Assert.Equal("Declined", updatedApplicant!.ApplicationStatus);
    }

    [Fact]
    public async Task SetInterviewDecision_ApplicantNotFound_SessionIsStillMarkedCompleted()
    {
        // No applicant seeded — should complete gracefully without throwing.
        interviewSessionRepository.Seed(SessionWithCandidate(sessionId: 1, positionId: 10, candidateId: 5));

        await interviewSessionService.SetInterviewDecision(sessionId: 1, decision: "Accepted");

        var session = await interviewSessionRepository.GetInterviewSessionByIdAsync(1);
        Assert.Equal(InterviewStatus.Completed.ToString(), session!.Status);
    }
}
