using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.Jobs;

public interface IJobService
{
    Task<Job?> GetByIdAsync(int jobId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Job>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);

    Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default);

    Task UpdateAsync(Job job, CancellationToken cancellationToken = default);

    /// <summary>Number of applicants (Match records) for a job, used to warn before deletion.</summary>
    Task<int> GetApplicantCountAsync(int jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a job. Applicants block deletion unless <paramref name="force"/> is <c>true</c>,
    /// in which case they are cascade-deleted along with the job.
    /// </summary>
    Task<JobDeleteResult> RemoveAsync(int jobId, bool force, CancellationToken cancellationToken = default);
}
