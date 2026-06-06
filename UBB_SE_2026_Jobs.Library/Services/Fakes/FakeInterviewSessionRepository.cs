using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

namespace UBB_SE_2026_Jobs.Tests.Fakes;

public class FakeInterviewSessionRepository : IInterviewSessionRepository
{
    private readonly List<InterviewSession> _store = new();
    private int _nextId = 1;

    public void Seed(params InterviewSession[] sessions)
    {
        foreach (var s in sessions)
        {
            _store.Add(s);
            if (s.Id >= _nextId)
                _nextId = s.Id + 1;
        }
    }

    public Task<List<InterviewSession>> GetScheduledSessionsAsync()
    {
        var results = _store
            .Where(s => s.Status == InterviewStatus.Scheduled.ToString())
            .ToList();
        return Task.FromResult(results);
    }

    public Task<List<InterviewSession>> GetSessionsByStatusAsync(string status)
    {
        var results = _store
            .Where(s => s.Status == status)
            .ToList();
        return Task.FromResult(results);
    }

    public Task<InterviewSession?> GetInterviewSessionByIdAsync(int id)
    {
        var session = _store.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(session);
    }

    public InterviewSession GetInterviewSessionById(int id) {
        var session = _store.FirstOrDefault(s => s.Id == id);
        return session;
    }

    public void Add(InterviewSession session)
    {
        if (session.Id == 0)
            session.Id = _nextId++;
        _store.Add(session);
    }

    public Task UpdateInterviewSessionAsync(InterviewSession session)
    {
        var index = _store.FindIndex(s => s.Id == session.Id);
        if (index >= 0)
            _store[index] = session;
        return Task.CompletedTask;
    }

    public void Delete(InterviewSession session)
    {
        _store.RemoveAll(s => s.Id == session.Id);
    }
}
