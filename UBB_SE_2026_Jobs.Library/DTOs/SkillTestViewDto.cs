using System;
using System.Collections.Generic;
using System.Text;

namespace UBB_SE_2026_Jobs.Library.DTOs;

/// <summary>
/// A read-only projection of a completed TestAttempt, shaped for display
/// consumers that previously used the SkillTest domain class.
/// The SkillTest table has been removed; this DTO is derived at query time
/// from TestAttempt joined with Test.
/// </summary>
public class SkillTestViewDto
{
    /// <summary>The TestAttempt.Id that backs this result.</summary>
    public int SkillTestId { get; init; }

    /// <summary>The title of the test (formerly SkillTest.Name).</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The integer percentage score (0-100).
    /// Derived from TestAttempt.PercentageScore, rounded.
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// The date the attempt was completed (formerly SkillTest.AchievedDate).
    /// </summary>
    public DateOnly AchievedDate { get; init; }

    /// <summary>The user this result belongs to.</summary>
    public int UserId { get; init; }
}