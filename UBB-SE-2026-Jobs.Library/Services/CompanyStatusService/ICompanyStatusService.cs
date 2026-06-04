using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Library.Services.CompanyStatusService;

public interface ICompanyStatusService
{
    Task<UserApplicationResult?> GetApplicantByMatchIdAsync(int companyId, int matchId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserApplicationResult>> GetApplicantsForCompanyAsync(int companyId, CancellationToken cancellationToken = default);
}
