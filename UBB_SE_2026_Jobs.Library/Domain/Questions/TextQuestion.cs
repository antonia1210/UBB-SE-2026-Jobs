namespace UBB_SE_2026_Jobs.Library.Domain.Questions
{
    using System.ComponentModel.DataAnnotations.Schema;
    using UBB_SE_2026_Jobs.Library.Domain.Core;

    /// <summary>
    /// TextQuestion class represents a specific type of question that requires a textual answer.
    /// It inherits from the base TestQuestion class and includes properties for the correct answer and the user's answer,
    /// both of which are not mapped to the database and can be used for temporary storage during the question creation
    /// or evaluation process.
    /// </summary>
    public class TextQuestion : TestQuestion
    {
        /// <summary>
        /// Gets or sets represents the correct answer for the text question. This property is not mapped to the database and can
        /// be used to temporarily store the correct answer during the question creation or editing process.
        /// </summary>
        [NotMapped]
        public string CorrectAnswerText
        {
            get => this.QuestionAnswer ?? string.Empty;
            set => this.QuestionAnswer = value;
        }

        /// <summary>
        /// Gets or sets represents the user's answer for the text question. This property is not mapped to the database and
        /// can be used to temporarily store the user's answer during the answering process. It allows for capturing the candidate's
        /// response to the text question, which can then be evaluated against the correct answer.
        /// </summary>
        [NotMapped]
        public string UserAnswerText { get; set; } = string.Empty;
    }
}
