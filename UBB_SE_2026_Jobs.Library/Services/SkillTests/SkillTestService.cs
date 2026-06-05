using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

namespace UBB_SE_2026_Jobs.Library.Services.SkillTests;

/// <summary>
/// Provides a display-oriented view of completed test results.
/// Previously read from the SkillTests table; now queries TestAttempts
/// (joined to Test for the title) so there is a single source of truth.
/// The SkillTests table and SkillTest domain class have been removed.
/// </summary>
public class SkillTestService : ISkillTestService
{
    private readonly ITestAttemptRepository testAttemptRepository;

    public SkillTestService(ITestAttemptRepository testAttemptRepository)
    {
        this.testAttemptRepository = testAttemptRepository;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SkillTestViewDto>> GetTestsForUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var attempts = await testAttemptRepository
            .FindCompletedByUserIdAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        return attempts
            .Select(ProjectToView)
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<SkillTestViewDto?> GetSkillTestByIdAsync(
        int skillTestId,
        CancellationToken cancellationToken = default)
    {
        var attempt = await testAttemptRepository
            .FindByIdAsync(skillTestId)
            .ConfigureAwait(false);

        if (attempt is null || !IsCompleted(attempt))
        {
            return null;
        }

        return ProjectToView(attempt);
    }

    private static bool IsCompleted(Domain.Core.TestAttempt attempt) =>
        attempt.Status == "COMPLETED" &&
        attempt.CompletedAt is not null;

    private static SkillTestViewDto ProjectToView(Domain.Core.TestAttempt attempt) =>
        new()
        {
            SkillTestId = attempt.Id,
            Name = attempt.Test?.Title ?? string.Empty,
            Score = (int)Math.Round(attempt.Score ?? 0m),
            AchievedDate = attempt.CompletedAt.HasValue
                               ? DateOnly.FromDateTime(attempt.CompletedAt.Value)
                               : DateOnly.MinValue,
            UserId = attempt.ExternalUserId ?? 0,
        };
}