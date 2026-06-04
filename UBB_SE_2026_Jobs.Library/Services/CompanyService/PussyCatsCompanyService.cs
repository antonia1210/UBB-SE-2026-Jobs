using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Helpers;
using UBB_SE_2026_Jobs.Library.Repositories.Companies;

namespace UBB_SE_2026_Jobs.Library.Services.PussyCatsCompanyService;

public class PussyCatsCompanyService : IPussyCatsCompanyService
{
    private readonly IPussyCatsCompanyRepository PussyCatsCompanyRepository;

    public PussyCatsCompanyService(IPussyCatsCompanyRepository PussyCatsCompanyRepository)
    {
        this.PussyCatsCompanyRepository = PussyCatsCompanyRepository;
    }

    public async Task<Company?> GetByIdAsync(int companyId, CancellationToken cancellationToken = default)//TODO
    {
        var company= await PussyCatsCompanyRepository.GetByIdAsync(companyId, cancellationToken).ConfigureAwait(false);
        DebugToFile.Write("Service","company is "+company);
        return company;
    }

    public async Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await PussyCatsCompanyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Company> AddAsync(Company company, CancellationToken cancellationToken = default)
    {
        return await PussyCatsCompanyRepository.AddAsync(company, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
    {
        await PussyCatsCompanyRepository.UpdateAsync(company, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(int companyId, CancellationToken cancellationToken = default)
    {
        await PussyCatsCompanyRepository.RemoveAsync(companyId, cancellationToken).ConfigureAwait(false);
    }
}
