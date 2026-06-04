namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Dtos
{
    /// <summary>
    /// Data transfer object for starting a test attempt.
    /// </summary>
    public class StartTestDto
    {
        /// <summary>Gets or sets the user identifier.</summary>
        public int UserId { get; set; }

        /// <summary>Gets or sets the test identifier.</summary>
        public int TestId { get; set; }
    }
}