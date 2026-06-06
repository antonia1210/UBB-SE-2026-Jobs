using AuthResponseDto = UBB_SE_2026_Jobs.Library.DTOs.AuthResponseDto;
using UBB_SE_2026_Jobs.Library.DTOs.Web;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public interface ITiAuthService
{
    Task<AuthResponseDto?> LoginAsync(string email, string password);
    Task<AuthResponseDto?> RegisterAsync(string name, string email, string password, string role, int? companyId = null);
    Task<List<CompanyDto>> GetCompaniesAsync();
}
