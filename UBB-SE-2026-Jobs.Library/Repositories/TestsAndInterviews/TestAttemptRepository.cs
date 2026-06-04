namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Data;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models.Core;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories.Interfaces;

    /// <inheritdoc cref="ITestAttemptRepository"/>
    public class TestAttemptRepository : ITestAttemptRepository
    {

        private readonly AppDbContext appDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAttemptRepository"/> class.
        /// TestAttemptRepository constructor initializes the connection string from the environment variable.
        /// </summary>
        public TestAttemptRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <inheritdoc />
        public async Task<TestAttempt?> FindByIdAsync(int id)
        {
            return await this.appDbContext.TestAttempts
                .Include(testAttempt => testAttempt.Answers)
                .ThenInclude(answer => answer.Question)
                .FirstOrDefaultAsync(testAttempt => testAttempt.Id == id);
        }

        /// <inheritdoc />
        public async Task<TestAttempt?> FindByUserAndTestAsync(int userId, int testId)
        {
            return await this.appDbContext.TestAttempts
                .Include(testAttempt => testAttempt.Answers)
                .FirstOrDefaultAsync(testAttempt => testAttempt.ExternalUserId == userId && testAttempt.TestId == testId);
        }

        /// <inheritdoc />
        public async Task SaveAsync(TestAttempt attempt)
        {
            this.appDbContext.TestAttempts.Add(attempt);
            await this.appDbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<TestAttempt?> UpdateAsync(TestAttempt attempt)
        {
            try
            {
                await this.appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await this.appDbContext.TestAttempts.AnyAsync(entry => entry.Id == attempt.Id))
                {
                    return null;
                }

                throw;
            }

            return attempt;
        }

        /// <inheritdoc />
        public async Task<List<TestAttempt>> FindValidAttemptsByTestIdAsync(int testId)
        {
            return await this.appDbContext.TestAttempts
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
    }
}