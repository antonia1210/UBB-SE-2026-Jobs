namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Dtos
{
    /// <summary>
    /// Data transfer object for submitting a test attempt with answers.
    /// </summary>
    public class SubmitAttemptDto
    {
        /// <summary>Gets or sets the user identifier.</summary>
        public int UserId { get; set; }

        /// <summary>Gets or sets the test identifier.</summary>
        public int TestId { get; set; }

        /// <summary>Gets or sets the list of answers.</summary>
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
    }
}