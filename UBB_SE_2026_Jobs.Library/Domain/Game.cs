namespace UBB_SE_2026_Jobs.Library.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Game class represents a game scenario where a user is presented with multiple scenarios and advice choices.
    /// It contains properties for the buddy associated with the game, a list of scenarios, a conclusion, and a flag indicating whether the game is published or not.
    /// The Game class provides methods to retrieve specific scenarios, add new scenarios, and manage the publication status of the game.
    /// </summary>
    public class Game
    {
        private const int DefaultBuddyId = 0;

        /// <summary>
        /// Gets or sets the unique identifier for this game.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the list of scenarios included in the game. Each scenario represents a situation or context in which the user is presented with multiple advice choices,
        /// allowing for an interactive and engaging gaming experience.
        /// </summary>
        public List<Scenario> Scenarios { get; set; } = new List<Scenario>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class with default values for the buddy, scenarios, conclusion, and publication status.
        /// </summary>
        public Game()
        {
            this.Buddy = new Buddy(DefaultBuddyId, string.Empty, string.Empty);
            this.Conclusion = string.Empty;
            this.IsPublished = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class with the specified buddy, list of scenarios, conclusion, and publication status.
        /// </summary>
        /// <param name="buddy">The buddy associated with the game.</param>
        /// <param name="scenarioList">The list of scenarios included in the game.</param>
        /// <param name="conclusion">The conclusion of the game.</param>
        /// <param name="isPublished">A value indicating whether the game is published or not.</param>
        /// <exception cref="ArgumentNullException">Thrown when the buddy or scenarioList is null.</exception>
        public Game(Buddy buddy, IEnumerable<Scenario> scenarioList, string conclusion, bool isPublished = false)
        {
            this.Buddy = buddy ?? throw new ArgumentNullException(nameof(buddy));
            this.Scenarios = scenarioList?.ToList() ?? throw new ArgumentNullException(nameof(scenarioList));
            this.Conclusion = conclusion ?? string.Empty;
            this.IsPublished = isPublished;
        }

        /// <summary>
        /// Gets the buddy associated with the game. This property represents the character or persona that the user interacts with during the game, providing context
        /// and guidance throughout the scenarios presented in the game.
        /// </summary>
        public Buddy Buddy { get; set; }

        /// <summary>
        /// Gets the conclusion of the game. This property represents the final outcome or summary of the game, providing closure and context for the scenarios and advice choices presented throughout the game.
        /// </summary>
        public string Conclusion { get; set; }

        /// <summary>
        /// Gets a value indicating whether the game is published or not. This property represents the publication status of the game, allowing for control over when the game is made available to users and when it is still in development or testing stages.
        /// </summary>
        public bool IsPublished { get; set; }


    }
}
