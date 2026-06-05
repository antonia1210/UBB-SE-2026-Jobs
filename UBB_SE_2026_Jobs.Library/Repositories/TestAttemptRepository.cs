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
        public async Task<TestAttempt?> FindByUserAndTestAsync(int userId, int testId)
        {
            return await databaseContext.TestAttempts
                .Include(testAttempt => testAttempt.Answers)
                .FirstOrDefaultAsync(testAttempt => testAttempt.ExternalUserId == userId && testAttempt.TestId == testId);
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
        public async Task<List<TestAttempt>> FindValidAttemptsByTestIdAsync(int testId)
        {
            return await databaseContext.TestAttempts
                .Include(testAttempt => testAttempt.User)
                .Where(testAttempt => testAttempt.TestId == testId
                         && testAttempt.Status == "COMPLETED"
                         && testAttempt.IsValidated
                         && testAttempt.PercentageScore != null
                         && testAttempt.CompletedAt != null)
                .OrderByDescending(testAttempt => testAttempt.PercentageScore)
                .ThenBy(testAttempt => testAttempt.CompletedAt)
                .ToListAsync();
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
                .Where(testAttempt => testAttempt.ExternalUserId == userId
                         && testAttempt.Status == "COMPLETED"
                         && testAttempt.CompletedAt != null)
                .OrderByDescending(testAttempt => testAttempt.CompletedAt)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}