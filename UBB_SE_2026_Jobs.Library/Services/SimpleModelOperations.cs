using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;

namespace UBB_SE_2026_Jobs.Library.Services;

public static class SimpleModelOperations
{
    public const float GoldScoreThreshold = 90f;
    public const float SilverScoreThreshold = 70f;
    public const float BronzeScoreThreshold = 50f;

    public const int GoldExperiencePoints = 100;
    public const int SilverExperiencePoints = 60;
    public const int BronzeExperiencePoints = 30;
    public const int ParticipantExperiencePoints = 10;

    public const int Level1ExperiencePoints = 0;
    public const int Level2ExperiencePoints = 100;
    public const int Level3ExperiencePoints = 250;
    public const int Level4ExperiencePoints = 500;
    public const int Level5ExperiencePoints = 800;

    public static readonly int[] LevelThresholds = new[]
    {
        Level1ExperiencePoints,
        Level2ExperiencePoints,
        Level3ExperiencePoints,
        Level4ExperiencePoints,
        Level5ExperiencePoints,
    };

    public static int CalculateLevelProgress(int xp, int level)
    {
        int bandIndex = Math.Clamp(level - 1, 0, LevelThresholds.Length - 2);
        int start = LevelThresholds[bandIndex];
        int end = LevelThresholds[bandIndex + 1];
        if (end <= start) return 100;
        return (int)Math.Clamp((xp - start) * 100.0 / (end - start), 0, 100);
    }

    public static int CalculateXpToNextLevel(int xp, int level)
    {
        if (level >= LevelThresholds.Length) return 0;
        return Math.Max(0, LevelThresholds[level] - xp);
    }

    public static int GetExperiencePoints(SkillTestViewDto skillTest)
    {
        if (skillTest.Score >= GoldScoreThreshold)
        {
            return GoldExperiencePoints;
        }

        if (skillTest.Score >= SilverScoreThreshold)
        {
            return SilverExperiencePoints;
        }

        if (skillTest.Score >= BronzeScoreThreshold)
        {
            return BronzeExperiencePoints;
        }

        return ParticipantExperiencePoints;
    }

    public static int CalculateLevelNumber(int experiencePoints)
    {
        for (int i = LevelThresholds.Length - 1; i >= 0; i--)
        {
            if (experiencePoints >= LevelThresholds[i])
            {
                return i + 1;
            }
        }
        return 1;
    }

    public static Badge AssignTier(float score)
    {
        switch (score)
        {
            case >= GoldScoreThreshold:
                return new Badge
                {
                    Tier = BadgeTier.Gold,
                    IconPath = "ms-appx:///Assets/badges/gold.svg",
                    ExperiencePointsValue = GoldExperiencePoints,
                };
            case >= SilverScoreThreshold:
                return new Badge
                {
                    Tier = BadgeTier.Silver,
                    IconPath = "ms-appx:///Assets/badges/silver.svg",
                    ExperiencePointsValue = SilverExperiencePoints,
                };
            case >= BronzeScoreThreshold:
                return new Badge
                {
                    Tier = BadgeTier.Bronze,
                    IconPath = "ms-appx:///Assets/badges/bronze.svg",
                    ExperiencePointsValue = BronzeExperiencePoints,
                };
            default:
                return new Badge
                {
                    Tier = BadgeTier.Participant,
                    IconPath = "ms-appx:///Assets/badges/participant.svg",
                    ExperiencePointsValue = ParticipantExperiencePoints,
                };
        }
    }
}
