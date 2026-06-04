namespace UBB_SE_2026_Jobs.Library.DTOs
{
    using System;

    /// <summary>
    /// Represents a job created by a company.
    /// </summary>
    public class JobDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the job.
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the company associated with the job.
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the photo associated with the job.
        /// </summary>
        public string? Photo { get; set; }

        /// <summary>
        /// Gets or sets the title of the job.
        /// </summary>
        public string? JobTitle { get; set; }

        /// <summary>
        /// Gets or sets the industry field of the job.
        /// </summary>
        public string? IndustryField { get; set; }

        /// <summary>
        /// Gets or sets the job type of the job.
        /// </summary>
        public string? JobType { get; set; }

        /// <summary>
        /// Gets or sets the experience level of the job.
        /// </summary>
        public string? ExperienceLevel { get; set; }

        /// <summary>
        /// Gets or sets the start date of the job.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the job.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the job description for the job.
        /// </summary>
        public string? JobDescription { get; set; }

        /// <summary>
        /// Gets or sets the job location for the job.
        /// </summary>
        public string? JobLocation { get; set; }

        /// <summary>
        /// Gets or sets the number of available positions for the job.
        /// </summary>
        public int AvailablePositions { get; set; }

        /// <summary>
        /// Gets or sets the time the job was made.
        /// </summary>
        public DateTime? PostedAt { get; set; }

        /// <summary>
        /// Gets or sets the salary for the job.
        /// </summary>
        public int? Salary { get; set; }

        /// <summary>
        /// Gets or sets the amount payed for the job.
        /// </summary>
        public int? AmountPayed { get; set; }

        /// <summary>
        /// Gets or sets the deadline for the job.
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Gets or sets the job skills fot the job.
        /// *Required skills: checkboxes with different skills options (Python, Java, C++, etc.) and a corresponding percentage representing the minimum required knowledge for the job;
        /// </summary>
        public System.Collections.Generic.ICollection<JobSkillDto> JobSkills { get; set; } = new System.Collections.Generic.List<JobSkillDto>();

    }
}