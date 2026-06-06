using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

namespace UBB_SE_2026_Jobs.Tests.Fakes;

public class FakeInterviewSessionRepository : IInterviewSessionRepository
{
    private readonly List<InterviewSession> interviewSessions = new();
    private int nextId = 1;

    public void Seed(params InterviewSession[] sessions)
    {
        foreach (var session in sessions)
        {
            interviewSessions.Add(session);
            if (session.Id >= nextId)
                nextId = session.Id + 1;
        }
    }

    public Task<List<InterviewSession>> GetScheduledSessionsAsync()
    {
        var results = interviewSessions
            .Where(interviewSession => interviewSession.Status == InterviewStatus.Scheduled.ToString())
            .ToList();
        return Task.FromResult(results);
    }

    public Task<List<InterviewSession>> GetSessionsByStatusAsync(string status)
    {
        var results = interviewSessions
            .Where(interviewSession => interviewSession.Status == status)
            .ToList();
        return Task.FromResult(results);
    }

    public Task<InterviewSession?> GetInterviewSessionByIdAsync(int id)
    {
        var session = interviewSessions.FirstOrDefault(interviewSession => interviewSession.Id == id);
        return Task.FromResult(session);
    }

    public InterviewSession GetInterviewSessionById(int id) {
        var session = interviewSessions.FirstOrDefault(interviewSession => interviewSession.Id == id);
        return session;
    }

    public void Add(InterviewSession session)
    {
        if (session.Id == 0)
            session.Id = nextId++;
        interviewSessions.Add(session);
    }

    public Task UpdateInterviewSessionAsync(InterviewSession session)
    {
        var index = interviewSessions.FindIndex(interviewSession => interviewSession.Id == session.Id);
        if (index >= 0)
            interviewSessions[index] = session;
        return Task.CompletedTask;
    }

    public void Delete(InterviewSession session)
    {
        interviewSessions.RemoveAll(interviewSession => interviewSession.Id == session.Id);
    }
}
