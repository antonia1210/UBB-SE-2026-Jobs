using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.Repositories.Skills;

public interface ISkillGroupRepository
{
    Task<IReadOnlyList<SkillGroup>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SkillGroup>> GetByJobRoleAsync(JobRole jobRole, CancellationToken cancellationToken = default);
}
