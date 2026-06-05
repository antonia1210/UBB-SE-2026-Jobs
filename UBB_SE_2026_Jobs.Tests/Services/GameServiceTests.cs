using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class GameServiceTests
{
    private readonly ITestsCompanyRepository testsCompanyRepository = Substitute.For<ITestsCompanyRepository>();
    private readonly GameService gameService;
    private const int makeGameBuddyId = 1;
    private const string makeGameBuddyName = "Alex", makeGameBuddyIntroduction = "Hi, I'm Alex.";
    private const string makeGameScenarioDescription = "You face a difficult situation.";
    private const string makeGameFirstAdvice = "Stay calm", makeGameFirstAdviceQuality = "Good choice.";
    private const string makeGameSecondAdvice = "Panic", makeGameSecondAdviceQuality = "Not ideal.";
    private const string makeGameConclusion = "Well done!";

    public GameServiceTests()
    {
        gameService = new GameService(testsCompanyRepository);
    }

    // Helper

    private static Game MakeGame(bool published = true)
    {
        var buddy = new Buddy(makeGameBuddyId, makeGameBuddyName, makeGameBuddyIntroduction);
        var scenario = new Scenario(makeGameScenarioDescription);
        scenario.AddChoice(new AdviceChoice(makeGameFirstAdvice, makeGameFirstAdviceQuality));
        scenario.AddChoice(new AdviceChoice(makeGameSecondAdvice, makeGameSecondAdviceQuality));

        return new Game(buddy, new List<Scenario> { scenario }, makeGameConclusion, published);
    }

    [Fact]
    public void LoadedGame_RepositoryReturnsNull_ThrowsInvalidOperationException()
    {
        testsCompanyRepository.GetGame().Returns((Game?)null);

        Assert.Throws<InvalidOperationException>(() => gameService.LoadedGame());
    }

    [Fact]
    public void LoadedGame_RepositoryReturnsGame_ReturnsIt()
    {
        var game = MakeGame();
        testsCompanyRepository.GetGame().Returns(game);

        var result = gameService.LoadedGame();

        Assert.Same(game, result);
    }


    [Fact]
    public void Save_NullGame_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => gameService.Save(null!));
    }


    [Fact]
    public void GetStoredGame_RepositoryReturnsNull_ReturnsNewGame()
    {
        testsCompanyRepository.GetGame().Returns((Game?)null);

        var result = gameService.GetStoredGame();

        Assert.NotNull(result);
    }

    [Fact]
    public void GetStoredGame_RepositoryReturnsGame_ReturnsThatGame()
    {
        var game = MakeGame();
        testsCompanyRepository.GetGame().Returns(game);

        var result = gameService.GetStoredGame();

        Assert.Same(game, result);
    }

    [Fact]
    public void IsPublished_RepositoryReturnsNull_ReturnsFalse()
    {
        testsCompanyRepository.GetGame().Returns((Game?)null);

        Assert.False(gameService.IsPublished());
    }

    [Fact]
    public void IsPublished_GameIsPublished_ReturnsTrue()
    {
        testsCompanyRepository.GetGame().Returns(MakeGame(published: true));

        Assert.True(gameService.IsPublished());
    }


    [Fact]
    public void ShowScenarioText_ValidIndex_ReturnsDescription()
    {
        int scenarioTextIndex = 0;
        testsCompanyRepository.GetGame().Returns(MakeGame());

        var result = gameService.ShowScenarioText(scenarioTextIndex);

        Assert.Equal(makeGameScenarioDescription, result);
    }

    [Fact]
    public void ShowScenarioText_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        testsCompanyRepository.GetGame().Returns(MakeGame());

        Assert.Throws<ArgumentOutOfRangeException>(() => gameService.ShowScenarioText(-1));
    }

    [Fact]
    public void ShowScenarioText_IndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        testsCompanyRepository.GetGame().Returns(MakeGame());

        Assert.Throws<ArgumentOutOfRangeException>(() => gameService.ShowScenarioText(1));
    }

    [Fact]
    public void ShowChoices_ValidIndex_ReturnsChoiceTexts()
    {
        int scenarioTextIndex = 0;
        int expectedNumberOfResults = 2;
        testsCompanyRepository.GetGame().Returns(MakeGame());

        var result = gameService.ShowChoices(scenarioTextIndex);

        Assert.Equal(expectedNumberOfResults, result.Count);
        Assert.Contains(makeGameFirstAdvice, result);
        Assert.Contains(makeGameSecondAdvice, result);
    }

    [Fact]
    public void ShowChoices_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        int invalidNegativeIndex = -1;
        testsCompanyRepository.GetGame().Returns(MakeGame());

        Assert.Throws<ArgumentOutOfRangeException>(() => gameService.ShowChoices(invalidNegativeIndex));
    }

    [Fact]
    public void ShowChoices_IndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        int indexEqualToLength = 1;
        testsCompanyRepository.GetGame().Returns(MakeGame());

        Assert.Throws<ArgumentOutOfRangeException>(() => gameService.ShowChoices(indexEqualToLength));
    }

    [Fact]
    public void ChoiceMade_ValidIndices_ReturnsFeedback()
    {
        int scenarioId = 0, adviceId = 0;
        testsCompanyRepository.GetGame().Returns(MakeGame());

        var result = gameService.ChoiceMade(scenarioId, adviceId);

        Assert.Equal(makeGameFirstAdviceQuality, result);
    }

    [Fact]
    public void ChoiceMade_NegativeScenarioIndex_ThrowsArgumentOutOfRangeException()
    {
        int invalidNegativeIndex = -1;
        int adviceId = 0;
        testsCompanyRepository.GetGame().Returns(MakeGame());

        Assert.Throws<ArgumentOutOfRangeException>(() => gameService.ChoiceMade(invalidNegativeIndex, adviceId));
    }

    [Fact]
    public void ChoiceMade_ScenarioIndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        int scenarioId = 1, adviceId = 0;
        testsCompanyRepository.GetGame().Returns(MakeGame());

        Assert.Throws<ArgumentOutOfRangeException>(() => gameService.ChoiceMade(scenarioId, adviceId));
    }

    [Fact]
    public void CreateGameFromInput_ValidInput_BuildsGameWithCorrectStructure()
    {
        int fixedBuddyId = 7;
        string buddyName = "Jordan", buddyIntroduction = "Hello!", conclusion = "Great job!";
        var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
        {
            ("Scenario one", new List<(string, string)>
            {
                ("Choice A", "Feedback A"),
                ("Choice B", "Feedback B"),
            }),
            ("Scenario two", new List<(string, string)>
            {
                ("Choice C", "Feedback C"),
            }),
        };

        var result = gameService.CreateGameFromInput(
            buddyId: fixedBuddyId,
            buddyName: buddyName,
            buddyIntroduction: buddyIntroduction,
            scenarios: scenarios,
            conclusion: conclusion,
            publish: true);

        Assert.Equal(fixedBuddyId, result.Buddy.Id);
        Assert.Equal(buddyName, result.Buddy.Name);
        Assert.Equal(buddyIntroduction, result.Buddy.Introduction);

        Assert.Equal(2, result.Scenarios.Count);
        Assert.Equal("Great job!", result.Conclusion);
        Assert.True(result.IsPublished);
    }

    [Fact]
    public void CreateGameFromInput_PublishFalse_GameIsNotPublished()
    {
        var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
        {
            ("Scenario", new List<(string, string)> { ("Choice", "Feedback") }),
        };

        var result = gameService.CreateGameFromInput(1, makeGameBuddyName, "Hi", scenarios, "Done", publish: false);

        Assert.False(result.IsPublished);
    }

    [Fact]
    public void CreateGameFromInput_EmptyScenarioList_BuildsGameWithNoScenarios()
    {
        int gameId = 1;
        var result = gameService.CreateGameFromInput(
            gameId, "Alex", "Hi",
            scenarios: new List<(string, IReadOnlyList<(string, string)>)>(),
            conclusion: "Done");

        Assert.Empty(result.Scenarios);
    }

    [Fact]
    public void CreateGameFromInput_ScenarioWithNoChoices_BuildsScenarioWithEmptyChoices()
    {
        int gameId = 1;
        var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
        {
            ("Empty scenario", new List<(string, string)>()),
        };

        var result = gameService.CreateGameFromInput(gameId, "Alex", "Hi", scenarios, "Done");

        Assert.Single(result.Scenarios);
        Assert.Empty(result.Scenarios[0].GetAdviceTexts());
    }
}