using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Library.Services.SkillTests;

public interface ISkillTestService
{
    /// <summary>
    /// Returns all completed, validated test attempts for the given user,
    /// projected as SkillTestViewDto for display purposes.
    /// </summary>
    Task<IReadOnlyList<SkillTestViewDto>> GetTestsForUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single completed test attempt projected as SkillTestViewDto, or null.
    /// </summary>
    Task<SkillTestViewDto?> GetSkillTestByIdAsync(int skillTestId, CancellationToken cancellationToken = default);
}
