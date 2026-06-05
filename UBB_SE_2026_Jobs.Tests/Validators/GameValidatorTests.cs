using System;
using System.Collections.Generic;
using Xunit;
using UBB_SE_2026_Jobs.Library.Validators;

namespace UBB_SE_2026_Jobs.Tests.Validators
{
    public class GameValidatorTests
    {
        

        private const string ValidScenarioText = "Valid scenario text";
        private const string ValidAdviceText = "Valid advice text";
        private const string ValidFeedbackText = "Valid feedback text";
        private const string ValidConclusion = "Valid conclusion";

        private const int StruggleOrAdviceLengthExceeded = 251;
        private const char FillerCharacter = 'a';

        private readonly IGameValidator gameValidator;

        public GameValidatorTests()
        {
            gameValidator = new GameValidator();
        }


        private const string ScenarioOneText = "First scenario";
        private const string ScenarioTwoText = "Second scenario";
        private const string AdviceOne = "Advice 1";
        private const string ReactionOne = "Reaction 1";
        private const string AdviceTwo = "Advice 2";
        private const string ReactionTwo = "Reaction 2";
        private const string AdviceThree = "Advice 3";
        private const string ReactionThree = "Reaction 3";

        private List<(string, IReadOnlyList<(string, string)>)> GetValidScenarios()
        {
            return new List<(string, IReadOnlyList<(string, string)>)>
    {
        (ScenarioOneText, new List<(string, string)>
        {
            (AdviceOne, ReactionOne),
            (AdviceTwo, ReactionTwo)
        }),
        (ScenarioTwoText, new List<(string, string)>
        {
            (AdviceThree, ReactionThree)
        })
    };
        }


        [Fact]
        public void ValidateMandatoryFields_LessThanRequiredScenarios_ThrowsException()
        {
            var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
            {
                (ScenarioOneText, new List<(string, string)> { (AdviceOne, ReactionOne) })
            };
            var exception = Assert.Throws<Exception>(() => gameValidator.ValidateMandatoryFields(scenarios));
        }


        [Fact]
        public void ValidateMandatoryFields_ScenarioWhitespaceText_ThrowsException()
        {
           
            var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
            {
                (ValidScenarioText, new List<(string, string)> { (ValidAdviceText, ValidFeedbackText) }),
                ("   ", new List<(string, string)> { (ValidAdviceText, ValidFeedbackText) })
            };

            var exception = Assert.Throws<Exception>(() => gameValidator.ValidateMandatoryFields(scenarios));
          
        }


        [Fact]
        public void ValidateMandatoryFields_ScenarioEmptyChoices_ThrowsException()
        {
            var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
            {
                (ValidScenarioText, new List<(string, string)> { (ValidAdviceText, ValidFeedbackText) }),
                (ValidScenarioText, new List<(string, string)>())
            };
            var exception = Assert.Throws<Exception>(() => gameValidator.ValidateMandatoryFields(scenarios));
           
        }


        [Fact]
        public void ValidateMandatoryFields_ChoiceNullAdviceText_ThrowsException()
        {
            var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
            {
                (ValidScenarioText, new List<(string, string)> { (null, ValidFeedbackText) }),
                (ValidScenarioText, new List<(string, string)> { (ValidAdviceText, ValidFeedbackText) })
            };
            var exception = Assert.Throws<Exception>(() => gameValidator.ValidateMandatoryFields(scenarios));
           
        }

        [Fact]
        public void ValidateMandatoryFields_ChoiceWhitespaceFeedbackText_ThrowsException()
        {
            var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
            {
                (ValidScenarioText, new List<(string, string)> { (ValidAdviceText, "   ") }),
                (ValidScenarioText, new List<(string, string)> { (ValidAdviceText, ValidFeedbackText) })
            };
            var exception = Assert.Throws<Exception>(() => gameValidator.ValidateMandatoryFields(scenarios));
          
        }

        [Fact]
        public void ValidateMandatoryFields_ValidScenarios_ReturnsTrue()
        {
            var result = gameValidator.ValidateMandatoryFields(GetValidScenarios());
            Assert.True(result);
        }


        [Fact]
        public void ValidateCharacterLimits_NullScenarios_ReturnsTrue()
        {
            var result = gameValidator.ValidateCharacterLimits(null);
            Assert.True(result);
        }

        [Fact]
        public void ValidateCharacterLimits_ScenarioTextExceedsMaxLength_ThrowsException()
        {
            var longText = new string(FillerCharacter, StruggleOrAdviceLengthExceeded);
            var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
            {
                (longText, new List<(string, string)> { (ValidAdviceText, ValidFeedbackText) }),
                (ValidScenarioText, new List<(string, string)> { (ValidAdviceText, ValidFeedbackText) })
            };
            var exception = Assert.Throws<Exception>(() => gameValidator.ValidateCharacterLimits(scenarios));
           
        }


        [Fact]
        public void ValidateCharacterLimits_AdviceTextExceedsMaxLength_ThrowsException()
        {
            var longAdvice = new string(FillerCharacter, StruggleOrAdviceLengthExceeded + 1);
            var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
            {
                (ValidScenarioText, new List<(string, string)> { (longAdvice, ValidFeedbackText) }),
                (ValidScenarioText, new List<(string, string)> { (ValidAdviceText, ValidFeedbackText) })
            };
            var exception = Assert.Throws<Exception>(() => gameValidator.ValidateCharacterLimits(scenarios));
          
        }

        [Fact]
        public void ValidateCharacterLimits_NullChoices_ReturnsTrue()
        {
            var scenarios = new List<(string, IReadOnlyList<(string, string)>)>
            {
                (ValidScenarioText, null),
                (ValidScenarioText, null)
            };
            var result = gameValidator.ValidateCharacterLimits(scenarios);
            Assert.True(result);
        }

        [Fact]
        public void ValidateCharacterLimits_ValidScenarios_ReturnsTrue()
        {
            var result = gameValidator.ValidateCharacterLimits(GetValidScenarios());
            Assert.True(result);
        }


        [Fact]
        public void ValidatePositiveConclusion_NullConclusion_ThrowsException()
        {
            var exception = Assert.Throws<Exception>(() => gameValidator.ValidatePositiveConclusion(null));
        }

        [Fact]
        public void ValidatePositiveConclusion_ValidConclusion_ReturnsTrue()
        {
            var result = gameValidator.ValidatePositiveConclusion(ValidConclusion);
            Assert.True(result);
        }

        [Fact]
        public void ValidateForActivation_ValidScenariosAndConclusion_ReturnsTrue()
        {
            var result = gameValidator.ValidateForActivation(GetValidScenarios(), ValidConclusion);
            Assert.True(result);
        }
    }
}