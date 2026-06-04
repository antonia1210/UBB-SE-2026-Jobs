namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    /// <inheritdoc cref="ILeaderboardRepository"/>
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly JobsDbContext JobsDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardRepository"/> class.
        /// </summary>
        public LeaderboardRepository(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> FindByTestIdAsync(int testId)
        {
            return await this.JobsDbContext.LeaderboardEntries
                .Include(le => le.User)
                .Include(le => le.Test)
                .Where(le => le.TestId == testId)
                .OrderBy(le => le.RankPosition)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> FindTopByTestIdAsync(int testId, int limit)
        {
            return await this.JobsDbContext.LeaderboardEntries
                .Include(le => le.User)
                .Include(le => le.Test)
                .Where(le => le.TestId == testId)
                .OrderBy(le => le.RankPosition)
                .Take(limit)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<LeaderboardEntry?> FindUserEntryAsync(int userId, int testId)
        {
            return await this.JobsDbContext.LeaderboardEntries
                .Include(le => le.User)
                .Include(le => le.Test)
                .FirstOrDefaultAsync(le => le.UserId == userId && le.TestId == testId);
        }

        /// <inheritdoc />
        public async Task DeleteByTestIdAsync(int testId)
        {
            var entries = await this.JobsDbContext.LeaderboardEntries
                .Where(le => le.TestId == testId)
                .ToListAsync();

            this.JobsDbContext.LeaderboardEntries.RemoveRange(entries);
            await this.JobsDbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveRangeAsync(List<LeaderboardEntry> entries)
        {
            if (entries == null || entries.Count == 0)
            {
                return;
            }

            await this.JobsDbContext.LeaderboardEntries.AddRangeAsync(entries);
            await this.JobsDbContext.SaveChangesAsync();
        }
    }
}

