using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Library.Services.CompatibilityService;

public interface ICompatibilityService
{
    Task<RoleResult> CalculateForRoleAsync(int userId, JobRole role, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoleResult>> CalculateAllAsync(int userId, CancellationToken cancellationToken = default);

    IReadOnlyList<Suggestion> GetSuggestions(RoleResult result);
}
