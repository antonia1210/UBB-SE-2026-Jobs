using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.CompanyService;

public interface ICompanyService
{
    Task<Company?> GetByIdAsync(int companyId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Company> AddAsync(Company company, CancellationToken cancellationToken = default);

    Task UpdateAsync(Company company, CancellationToken cancellationToken = default);

    Task RemoveAsync(int companyId, CancellationToken cancellationToken = default);
}
