
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.Skills;

public interface ISkillService
{
    Task<IReadOnlyList<Skill>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Skill?> GetByIdAsync(int skillId, CancellationToken cancellationToken = default);
    Task<Skill> AddAsync(Skill skill, CancellationToken cancellationToken = default);
    Task UpdateAsync(Skill skill, CancellationToken cancellationToken = default);
    Task RemoveAsync(int skillId, CancellationToken cancellationToken = default);
}
