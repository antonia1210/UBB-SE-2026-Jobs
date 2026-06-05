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
        private readonly JobsDbContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardRepository"/> class.
        /// </summary>
        public LeaderboardRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> FindByTestIdAsync(int testId)
        {
            return await this.databaseContext.LeaderboardEntries
                .Include(leaderboardEntry => leaderboardEntry.User)
                .Include(leaderboardEntry => leaderboardEntry.Test)
                .Where(leaderboardEntry => leaderboardEntry.TestId == testId)
                .OrderBy(leaderboardEntry => leaderboardEntry.RankPosition)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> FindTopByTestIdAsync(int testId, int limit)
        {
            return await this.databaseContext.LeaderboardEntries
                .Include(leaderboardEntry => leaderboardEntry.User)
                .Include(leaderboardEntry => leaderboardEntry.Test)
                .Where(leaderboardEntry => leaderboardEntry.TestId == testId)
                .OrderBy(leaderboardEntry => leaderboardEntry.RankPosition)
                .Take(limit)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<LeaderboardEntry?> FindUserEntryAsync(int userId, int testId)
        {
            return await this.databaseContext.LeaderboardEntries
                .Include(leaderboardEntry => leaderboardEntry.User)
                .Include(leaderboardEntry => leaderboardEntry.Test)
                .FirstOrDefaultAsync(leaderboardEntry => leaderboardEntry.UserId == userId && leaderboardEntry.TestId == testId);
        }

        /// <inheritdoc />
        public async Task DeleteByTestIdAsync(int testId)
        {
            var entries = await this.databaseContext.LeaderboardEntries
                .Where(leaderboardEntry => leaderboardEntry.TestId == testId)
                .ToListAsync();

            this.databaseContext.LeaderboardEntries.RemoveRange(entries);
            await this.databaseContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveRangeAsync(List<LeaderboardEntry> leaderboardEntries)
        {
            if (leaderboardEntries == null || leaderboardEntries.Count == 0)
            {
                return;
            }

            await this.databaseContext.LeaderboardEntries.AddRangeAsync(leaderboardEntries);
            await this.databaseContext.SaveChangesAsync();
        }
    }
}