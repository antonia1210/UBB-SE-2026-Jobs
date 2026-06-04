using System.Collections.Generic;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.Interfaces
{
    public interface IGameService
    {
        Game LoadedGame();
        int GetBuddyId();
        void Save(Game game);
        Game GetStoredGame();
        bool IsPublished();
        string ShowCoworker();
        string ShowScenarioText(int number);
        List<string> ShowChoices(int number);
        string ChoiceMade(int numberScenario, int numberAdvice);
        string ShowConclusion();
        Game CreateGameFromInput(
            int buddyId,
            string buddyName,
            string buddyIntroduction,
            IReadOnlyList<(string scenarioText, IReadOnlyList<(string advice, string feedback)> choices)> scenarios,
            string conclusion,
            bool publish = true);
        void PublishGame(Game existingGame);
        void UnpublishGame(Game existingGame);
    }
}
