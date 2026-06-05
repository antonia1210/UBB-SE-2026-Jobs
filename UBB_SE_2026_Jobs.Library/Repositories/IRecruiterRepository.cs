namespace UBB_SE_2026_Jobs.Library.Repositories;

public interface IRecruiterRepository
{
    Task<IReadOnlyList<int>> GetUserIdsByCompanyAsync(int companyId, CancellationToken cancellationToken = default);
}
