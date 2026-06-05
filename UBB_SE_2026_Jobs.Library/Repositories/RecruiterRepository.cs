using Microsoft.EntityFrameworkCore;
using UBB_SE_2026_Jobs.Library.Persistence;

namespace UBB_SE_2026_Jobs.Library.Repositories;

public class RecruiterRepository : IRecruiterRepository
{
    private readonly JobsDbContext databaseContext;

    public RecruiterRepository(JobsDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<int>> GetUserIdsByCompanyAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.Recruiters
            .Where(r => r.CompanyId == companyId)
            .Select(r => r.UserId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<int>> GetAllRecruiterUserIdsAsync(CancellationToken cancellationToken = default)
    {
        var ids = await databaseContext.Recruiters
            .Select(r => r.UserId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return new HashSet<int>(ids);
    }

    public async Task<int?> GetCompanyIdForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.Recruiters
            .Where(r => r.UserId == userId)
            .Select(r => (int?)r.CompanyId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
