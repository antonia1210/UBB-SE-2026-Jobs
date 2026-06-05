using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Library.Services;

public static class UserLevelService
{
    private const int FullProgressPercentage = 100;
    private const int NoExperiencePointsRemaining = 0;

    private const int MinimumExperiencePoints = 0;

    private const int Level1Number = 1;
    private const int Level2Number = 2;
    private const int Level3Number = 3;
    private const int Level4Number = 4;
    private const int MaximumLevelNumber = 5;

    public static int GetLevelProgressPercent(int totalExperiencePoints, int currentLevel)
    {
        if (totalExperiencePoints < MinimumExperiencePoints)
        {
            throw new ArgumentException("Experience Points cannot be negative.");
        }

        int nextLevelExperiencePoints = GetNextLevelExperiencePoints(currentLevel);
        if (nextLevelExperiencePoints == SimpleModelOperations.Level1ExperiencePoints)
        {
            return FullProgressPercentage;
        }

        double completedPercentageIntoCurrentLevel = GetLevelProgressPercentage(totalExperiencePoints, currentLevel);
        return (int)completedPercentageIntoCurrentLevel;
    }

    private static double GetLevelProgressPercentage(int totalExperiencePoints, int currentLevel)
    {
        int experiencePointsRequired = GetExperiencePointsRequiredForLevel(currentLevel);
        int nextLevelExperiencePoints = GetNextLevelExperiencePoints(currentLevel);
        double pointsIntoLevel = totalExperiencePoints - experiencePointsRequired;
        double totalPointsForLevel = nextLevelExperiencePoints - experiencePointsRequired;
        double completedPercentageIntoCurrentLevel = pointsIntoLevel / totalPointsForLevel * FullProgressPercentage;
        return completedPercentageIntoCurrentLevel;
    }

    public static int GetExperiencePointsToNextLevel(int totalExperiencePoints, int currentLevel)
    {
        if (totalExperiencePoints < MinimumExperiencePoints)
        {
            throw new ArgumentException("Experience Points cannot be negative.");
        }

        int nextLevelExperiencePoints = GetNextLevelExperiencePoints(currentLevel);
        if (nextLevelExperiencePoints == SimpleModelOperations.Level1ExperiencePoints)
        {
            return NoExperiencePointsRemaining;
        }
        return nextLevelExperiencePoints - totalExperiencePoints;
    }

    public static int CalculateLevelNumber(int experiencePoints)
    {
        if (experiencePoints < MinimumExperiencePoints)
        {
            throw new ArgumentException("Experience Points cannot be negative.");
        }
        return SimpleModelOperations.CalculateLevelNumber(experiencePoints);
    }

    public static int GetExperiencePointsRequiredForLevel(int level)
    {
        return level switch
        {
            >= MaximumLevelNumber => SimpleModelOperations.Level5ExperiencePoints,
            Level4Number => SimpleModelOperations.Level4ExperiencePoints,
            Level3Number => SimpleModelOperations.Level3ExperiencePoints,
            Level2Number => SimpleModelOperations.Level2ExperiencePoints,
            _ => SimpleModelOperations.Level1ExperiencePoints,
        };
    }

    public static int GetNextLevelExperiencePoints(int currentLevel)
    {
        return currentLevel switch
        {
            >= MaximumLevelNumber => SimpleModelOperations.Level1ExperiencePoints,
            Level4Number => SimpleModelOperations.Level5ExperiencePoints,
            Level3Number => SimpleModelOperations.Level4ExperiencePoints,
            Level2Number => SimpleModelOperations.Level3ExperiencePoints,
            _ => SimpleModelOperations.Level2ExperiencePoints,
        };
    }
}
