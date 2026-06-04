namespace UBB_SE_2026_Jobs.Library.DTOs
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the data required to add a new job.
    /// </summary>
    public class AddJobDto
    {
        /// <summary>
        /// Gets or sets the job data.
        /// </summary>
        public JobDto Job { get; set; } = null!;

        /// <summary>
        /// Gets or sets the unique identifier of the user adding the job.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the list of skill links associated with the job.
        /// </summary>
        public List<JobSkillDto> SkillLinks { get; set; } = new List<JobSkillDto>();
    }
}