namespace UBB_SE_2026_Jobs.Library.Domain
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Scenario class represents a situation or context in which a user is presented with multiple advice choices.
    /// It contains a description of the scenario and a list of advice choices that the user can select from.
    /// Each advice choice includes the advice text and feedback for that choice.
    /// The Scenario class provides methods to retrieve the advice texts, feedback, and to select a specific choice based on an index.
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// Gets or sets the unique identifier for this scenario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the list of advice choices available in this scenario. Each advice choice includes the advice text and feedback for that choice.
        /// </summary>
        public List<AdviceChoice> Choices { get; set; } = new List<AdviceChoice>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario"/> class.
        /// Required for Entity Framework.
        /// </summary>
        public Scenario()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario"/> class with the specified description.
        /// The constructor also initializes the list of advice choices to an empty list, allowing for advice choices to be added later using the AddChoice method.
        /// </summary>
        /// <param name="description">The description of the scenario, providing context for the advice choices presented to the user.</param>
        public Scenario(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Gets the description of the scenario, providing context for the advice choices presented to the user.
        /// This property is read-only and is set through the constructor when creating a new instance of the Scenario class.
        /// </summary>
        public string Description { get; set; }


    }
}
