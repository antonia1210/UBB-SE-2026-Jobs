using Microsoft.EntityFrameworkCore;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Persistence;

namespace UBB_SE_2026_Jobs.Library.Repositories.Jobs;

public class PussyCatsJobRepository : IPussyCatsJobRepository
{
    private readonly JobsDbContext databaseContext;

    public PussyCatsJobRepository(JobsDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    /// <summary>
    /// Includes Company and RequiredSkills.Skill so a job-detail screen has everything it needs
    /// to render. Tracked because the typical caller (recruiter editing a posting) mutates.
    /// </summary>
    public async Task<Job?> GetByIdAsync(int jobId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.Jobs
            .Include(job => job.Company)
            .Include(job => job.RequiredSkills).ThenInclude(jobSkill => jobSkill.Skill)
            .FirstOrDefaultAsync(job => job.JobId == jobId, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Browse-jobs listing — includes Company so the listing card can show the employer name
    /// without an N+1.
    /// </summary>
    public async Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await databaseContext.Jobs
            .AsNoTracking()
            .Include(job => job.Company)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Original: matchmaking PussyCatsJobRepository.GetByCompanyId — straight LINQ port of the foreach
    /// filter on CompanyId. Read-only, no Includes (callers already have the Company).
    /// </summary>
    public async Task<IReadOnlyList<Job>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.Jobs
            .AsNoTracking()
            .Where(job => job.Company.CompanyId == companyId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        databaseContext.Jobs.Add(job);
        await databaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return job;
    }

    public async Task UpdateAsync(Job job, CancellationToken cancellationToken = default)
    {
        var trackedJob = databaseContext.Jobs.Local.FirstOrDefault(existingJob => existingJob.JobId == job.JobId);
        if (trackedJob is not null)
        {
            databaseContext.Entry(trackedJob).CurrentValues.SetValues(job);
        }
        else
        {
            databaseContext.Entry(job).State = EntityState.Modified;
        }
        await databaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(int jobId, CancellationToken cancellationToken = default)
    {
        var job = await databaseContext.Jobs.FindAsync(new object?[] { jobId }, cancellationToken).ConfigureAwait(false);
        if (job is null)
        {
            return;
        }
        databaseContext.Jobs.Remove(job);
        await databaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}