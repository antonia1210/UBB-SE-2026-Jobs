using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Repositories;

public interface IRecruiterRepository
{
    Task<IReadOnlyList<int>> GetUserIdsByCompanyAsync(int companyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<int>> GetAllRecruiterUserIdsAsync(CancellationToken cancellationToken = default);
    Task<int?> GetCompanyIdForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Recruiter recruiter, CancellationToken cancellationToken = default);
}
