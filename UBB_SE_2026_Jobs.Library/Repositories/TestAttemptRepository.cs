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

        private readonly JobsDbContext jobsDbContext;

        public TestAttemptRepository(JobsDbContext jobsDbContext)
        {
            this.jobsDbContext = jobsDbContext;
        }

        /// <inheritdoc/>
        public async Task<TestAttempt?> FindByIdAsync(int id)
        {
            return await jobsDbContext.TestAttempts
                .Include(a => a.Answers)
                    .ThenInclude(answer => answer.Question)
                .Include(a => a.Test)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <inheritdoc/>
        public async Task<TestAttempt?> FindByUserAndTestAsync(int userId, int testId)
        {
            return await jobsDbContext.TestAttempts
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.ExternalUserId == userId && a.TestId == testId);
        }

        /// <inheritdoc/>
        public async Task SaveAsync(TestAttempt attempt)
        {
            jobsDbContext.TestAttempts.Add(attempt);
            await jobsDbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<TestAttempt?> UpdateAsync(TestAttempt attempt)
        {
            try
            {
                await jobsDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await jobsDbContext.TestAttempts.AnyAsync(e => e.Id == attempt.Id))
                {
                    return null;
                }
                throw;
            }

            return attempt;
        }

        /// <inheritdoc/>
        public async Task<List<TestAttempt>> FindValidAttemptsByTestIdAsync(int testId)
        {
            return await jobsDbContext.TestAttempts
                .Include(a => a.User)
                .Where(a => a.TestId == testId
                         && a.Status == "COMPLETED"
                         && a.IsValidated
                         && a.PercentageScore != null
                         && a.CompletedAt != null)
                .OrderByDescending(a => a.PercentageScore)
                .ThenBy(a => a.CompletedAt)
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
            return await jobsDbContext.TestAttempts
                .AsNoTracking()
                .Include(a => a.Test)
                .Where(a => a.ExternalUserId == userId
                         && a.Status == "COMPLETED"
                         && a.CompletedAt != null)
                .OrderByDescending(a => a.CompletedAt)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}

