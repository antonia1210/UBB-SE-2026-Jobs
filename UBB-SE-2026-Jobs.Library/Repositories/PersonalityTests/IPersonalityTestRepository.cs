using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Repositories.PersonalityTests;

public interface IPersonalityTestRepository
{
    Task<PersonalityTestResult?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<PersonalityTestResult> AddAsync(PersonalityTestResult result, CancellationToken cancellationToken = default);

    Task UpdateAsync(PersonalityTestResult result, CancellationToken cancellationToken = default);

    Task RemoveAsync(int personalityTestResultId, CancellationToken cancellationToken = default);
}
