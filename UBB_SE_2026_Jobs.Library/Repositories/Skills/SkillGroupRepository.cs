using Microsoft.EntityFrameworkCore;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Persistence;

namespace UBB_SE_2026_Jobs.Library.Repositories.Skills;

public class SkillGroupRepository : ISkillGroupRepository
{
    private readonly JobsDbContext databaseContext;

    public SkillGroupRepository(JobsDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    /// <summary>
    /// Read-only listing â€” includes Skills so CompatibilityService can score without N+1
    /// fetches against the catalog.
    /// </summary>
    public async Task<IReadOnlyList<SkillGroup>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await databaseContext.SkillGroups
            .AsNoTracking()
            .Include(group => group.Skills)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Original: PussyCatsApp SkillGroupRepository.GetSkillsGroupByRole â€” straight predicate
    /// port. CompatibilityService scores against this list per role; the Skills include is the
    /// whole point of the call.
    /// </summary>
    public async Task<IReadOnlyList<SkillGroup>> GetByJobRoleAsync(JobRole jobRole, CancellationToken cancellationToken = default)
    {
        return await databaseContext.SkillGroups
            .AsNoTracking()
            .Include(group => group.Skills)
            .Where(group => group.JobRole == jobRole)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

