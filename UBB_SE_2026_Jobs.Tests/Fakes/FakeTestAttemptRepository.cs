using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

namespace UBB_SE_2026_Jobs.Tests.Fakes;

public class FakeTestAttemptRepository : ITestAttemptRepository
{
    private readonly List<TestAttempt> attempts = new();

    public void Seed(params TestAttempt[] items) => attempts.AddRange(items);

    public Task<TestAttempt?> FindByIdAsync(int id) =>
        Task.FromResult(attempts.FirstOrDefault(testAttempt => testAttempt.Id == id));

    public Task<TestAttempt?> FindByUserAndTestAsync(int userId, int testId) =>
        Task.FromResult(attempts.FirstOrDefault(testAttempt => testAttempt.ExternalUserId == userId && testAttempt.TestId == testId));

    public Task SaveAsync(TestAttempt attempt)
    {
        attempts.Add(attempt);
        return Task.CompletedTask;
    }

    public Task<TestAttempt?> UpdateAsync(TestAttempt attempt)
    {
        var existing = attempts.FirstOrDefault(testAttempt => testAttempt.Id == attempt.Id);
        if (existing is null) return Task.FromResult<TestAttempt?>(null);
        attempts.Remove(existing);
        attempts.Add(attempt);
        return Task.FromResult<TestAttempt?>(attempt);
    }

    public Task<List<TestAttempt>> FindValidAttemptsByTestIdAsync(int testId) =>
        Task.FromResult(attempts
            .Where(testAttempt => testAttempt.TestId == testId && testAttempt.Status == "COMPLETED" && testAttempt.IsValidated)
            .ToList());

    public Task<IReadOnlyList<TestAttempt>> FindCompletedByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<TestAttempt>>(attempts
            .Where(testAttempt => testAttempt.ExternalUserId == userId
                     && testAttempt.Status == "COMPLETED"
                     && testAttempt.IsValidated
                     && testAttempt.CompletedAt != null)
            .ToList());
}