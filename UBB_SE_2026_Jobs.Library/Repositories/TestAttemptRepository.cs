namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    /// <inheritdoc cref="ITestAttemptRepository"/>
    public class TestAttemptRepository : ITestAttemptRepository
    {
        private readonly JobsDbContext databaseContext;

        public TestAttemptRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <inheritdoc/>
        public async Task<TestAttempt?> FindByIdAsync(int testAttemptId)
        {
            return await databaseContext.TestAttempts
                .Include(testAttempt => testAttempt.Answers)
                    .ThenInclude(answer => answer.Question)
                .Include(testAttempt => testAttempt.Test)
                .FirstOrDefaultAsync(testAttempt => testAttempt.Id == testAttemptId);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Returns the most recent IN_PROGRESS attempt for the user/test pair.
        /// Tests are replayable; a user may have multiple attempts. Submission flow
        /// always targets the current active attempt, not a previously completed one.
        /// </remarks>
        public async Task<TestAttempt?> FindByUserAndTestAsync(int userId, int testId)
        {
            return await databaseContext.TestAttempts
                .Include(testAttempt => testAttempt.Answers)
                .Where(testAttempt => testAttempt.ExternalUserId == userId
                                   && testAttempt.TestId == testId
                                   && testAttempt.Status == "IN_PROGRESS")
                .OrderByDescending(testAttempt => testAttempt.StartedAt)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task SaveAsync(TestAttempt testAttempt)
        {
            databaseContext.TestAttempts.Add(testAttempt);
            await databaseContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<TestAttempt?> UpdateAsync(TestAttempt testAttempt)
        {
            if (databaseContext.Entry(testAttempt).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
            {
                databaseContext.TestAttempts.Update(testAttempt);
            }

            try
            {
                await databaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await databaseContext.TestAttempts.AnyAsync(existingAttempt => existingAttempt.Id == testAttempt.Id))
                {
                    return null;
                }
                throw;
            }

            return testAttempt;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Returns the single best attempt per user for leaderboard ranking.
        /// With replayability a user can have multiple COMPLETED attempts; only their
        /// highest PercentageScore (earliest CompletedAt on tie) enters the board.
        /// </remarks>
        public async Task<List<TestAttempt>> FindValidAttemptsByTestIdAsync(int testId)
        {
            var all = await databaseContext.TestAttempts
                .Include(testAttempt => testAttempt.User)
                .Where(testAttempt => testAttempt.TestId == testId
                         && testAttempt.Status == "COMPLETED"
                         && testAttempt.IsValidated
                         && testAttempt.PercentageScore != null
                         && testAttempt.CompletedAt != null)
                .ToListAsync();

            return all
                .GroupBy(a => a.ExternalUserId)
                .Select(g => g
                    .OrderByDescending(a => a.PercentageScore)
                    .ThenBy(a => a.CompletedAt)
                    .First())
                .OrderByDescending(a => a.PercentageScore)
                .ThenBy(a => a.CompletedAt)
                .ToList();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Replaces the former SkillTestRepository.GetByUserIdAsync.
        /// Includes the Test navigation so callers can read Test.Title
        /// without a second round-trip (previously stored as SkillTest.Name).
        /// </remarks>
        public async Task<IReadOnlyList<TestAttempt>> FindCompletedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await databaseContext.TestAttempts
                .AsNoTracking()
                .Include(testAttempt => testAttempt.Test)
                    .ThenInclude(test => test!.Questions)
                .Include(testAttempt => testAttempt.Test)
                    .ThenInclude(test => test!.Skill)
                .Where(testAttempt => testAttempt.ExternalUserId == userId
                         && testAttempt.Status == "COMPLETED"
                         && testAttempt.CompletedAt != null)
                .OrderByDescending(testAttempt => testAttempt.CompletedAt)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}