namespace UBB_SE_2026_Jobs.Library.Repositories.Portal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;
    using UBB_SE_2026_Jobs.Library.Repositories.Portal;

    /// <inheritdoc cref="ILeaderboardRepository"/>
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly PortalDbContext appDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardRepository"/> class.
        /// </summary>
        public LeaderboardRepository(PortalDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> FindByTestIdAsync(int testId)
        {
            return await this.appDbContext.LeaderboardEntries
                .Include(le => le.User)
                .Include(le => le.Test)
                .Where(le => le.TestId == testId)
                .OrderBy(le => le.RankPosition)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> FindTopByTestIdAsync(int testId, int limit)
        {
            return await this.appDbContext.LeaderboardEntries
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
            return await this.appDbContext.LeaderboardEntries
                .Include(le => le.User)
                .Include(le => le.Test)
                .FirstOrDefaultAsync(le => le.UserId == userId && le.TestId == testId);
        }

        /// <inheritdoc />
        public async Task DeleteByTestIdAsync(int testId)
        {
            var entries = await this.appDbContext.LeaderboardEntries
                .Where(le => le.TestId == testId)
                .ToListAsync();

            this.appDbContext.LeaderboardEntries.RemoveRange(entries);
            await this.appDbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveRangeAsync(List<LeaderboardEntry> entries)
        {
            if (entries == null || entries.Count == 0)
            {
                return;
            }

            await this.appDbContext.LeaderboardEntries.AddRangeAsync(entries);
            await this.appDbContext.SaveChangesAsync();
        }
    }
}