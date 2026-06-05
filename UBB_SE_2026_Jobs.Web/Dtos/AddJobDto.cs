namespace UBB_SE_2026_Jobs.Web.Dtos
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the data required to add a new job posting.
    /// </summary>
    public class AddJobDto
    {
        /// <summary>
        /// Gets or sets the job posting data. The property name matches the
        /// merged API's AddJobDto contract.
        /// </summary>
        public JobPostingDto Job { get; set; } = null!;

        /// <summary>
        /// Gets or sets the unique identifier of the user adding the job.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the list of skill links associated with the job posting.
        /// </summary>
        public List<JobSkillDto> SkillLinks { get; set; } = new List<JobSkillDto>();
    }
}
