using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Repositories.Jobs;

namespace UBB_SE_2026_Jobs.Library.Services.Jobs;

public class PussyCatsJobService : IPussyCatsJobService
{
    private readonly IPussyCatsJobRepository PussyCatsJobRepository;

    public PussyCatsJobService(IPussyCatsJobRepository PussyCatsJobRepository)
    {
        this.PussyCatsJobRepository = PussyCatsJobRepository;
    }

    public async Task<Job?> GetByIdAsync(int jobId, CancellationToken cancellationToken = default)
    {
        return await PussyCatsJobRepository.GetByIdAsync(jobId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await PussyCatsJobRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Job>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await PussyCatsJobRepository.GetByCompanyIdAsync(companyId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        return await PussyCatsJobRepository.AddAsync(job, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Job job, CancellationToken cancellationToken = default)
    {
        await PussyCatsJobRepository.UpdateAsync(job, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(int jobId, CancellationToken cancellationToken = default)
    {
        await PussyCatsJobRepository.RemoveAsync(jobId, cancellationToken).ConfigureAwait(false);
    }
}
