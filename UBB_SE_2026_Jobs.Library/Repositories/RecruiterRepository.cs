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
            .Where(recruiter => recruiter.CompanyId == companyId)
            .Select(recruiter => recruiter.UserId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<int>> GetAllRecruiterUserIdsAsync(CancellationToken cancellationToken = default)
    {
        var recruiterUserIds = await databaseContext.Recruiters
            .Select(recruiter => recruiter.UserId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return new HashSet<int>(recruiterUserIds);
    }

    public async Task<int?> GetCompanyIdForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.Recruiters
            .Where(recruiter => recruiter.UserId == userId)
            .Select(recruiter => (int?)recruiter.CompanyId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}