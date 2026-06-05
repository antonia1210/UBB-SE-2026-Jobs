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
    public const int Level2ExperiencePoints = 10;
    public const int Level3ExperiencePoints = 250;
    public const int Level4ExperiencePoints = 500;
    public const int Level5ExperiencePoints = 800;

    private const int Level1Number = 1;
    private const int Level2Number = 2;
    private const int Level3Number = 3;
    private const int Level4Number = 4;
    private const int Level5Number = 5;

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
        if (experiencePoints >= Level5ExperiencePoints)
        {
            return Level5Number;
        }

        if (experiencePoints >= Level4ExperiencePoints)
        {
            return Level4Number;
        }

        if (experiencePoints >= Level3ExperiencePoints)
        {
            return Level3Number;
        }

        if (experiencePoints >= Level2ExperiencePoints)
        {
            return Level2Number;
        }

        return Level1Number;
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
