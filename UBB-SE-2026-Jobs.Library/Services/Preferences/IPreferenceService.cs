using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Library.Services.Preferences;

/// <summary>Reads and writes the three user preference fields: employment type, work mode, location.</summary>
public interface IPreferenceService
{
    Task<UserPreferences> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task SavePreferencesAsync(int userId, IReadOnlyList<JobRole> roles, WorkMode workMode, string location, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> SearchLocationsAsync(string locationQuery, CancellationToken cancellationToken = default);
}
