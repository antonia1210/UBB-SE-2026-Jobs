using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.DTOs;

public sealed record UserPreferences(
    IReadOnlyList<JobRole> Roles,
    WorkMode WorkMode,
    string Location);
