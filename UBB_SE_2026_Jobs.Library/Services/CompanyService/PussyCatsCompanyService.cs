using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Repositories.Companies;
using UBB_SE_2026_Jobs.Library.Services.PussyCatsCompanyService;

namespace UBB_SE_2026_Jobs.Library.Services.CompanyService;

public class PussyCatsCompanyService : IPussyCatsCompanyService
{
    private readonly IPussyCatsCompanyRepository companyRepository;

    public PussyCatsCompanyService(IPussyCatsCompanyRepository companyRepository)
    {
        this.companyRepository = companyRepository;
    }

    public async Task<Company?> GetByIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await companyRepository.GetByIdAsync(companyId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await companyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Company> AddAsync(Company company, CancellationToken cancellationToken = default)
    {
        return await companyRepository.AddAsync(company, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
    {
        await companyRepository.UpdateAsync(company, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(int companyId, CancellationToken cancellationToken = default)
    {
        await companyRepository.RemoveAsync(companyId, cancellationToken).ConfigureAwait(false);
    }
}