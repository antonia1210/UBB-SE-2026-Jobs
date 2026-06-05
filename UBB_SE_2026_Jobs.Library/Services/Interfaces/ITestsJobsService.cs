namespace UBB_SE_2026_Jobs.Library.Services.Interfaces
{
    using System.Collections.Generic;
    using UBB_SE_2026_Jobs.Library.Domain;

    /// <summary>
    /// Defines operations for managing job postings.
    /// </summary>
    public interface ITestsJobsService
    {
        /// <summary>
        /// Retrieves all job postings.
        /// </summary>
        /// <returns>A list of all job postings.</returns>
        IEnumerable<Job> GetAllJobs();

        /// <summary>
        /// Retrieves all available skills.
        /// </summary>
        /// <returns>A read-only list of all skills.</returns>
        IReadOnlyList<Skill> GetAllSkills();

        /// <summary>
        /// Adds a new job posting to the data store.
        /// </summary>
        /// <param name="Job">The job posting to add. Cannot be null.</param>
        /// <param name="companyId">The unique identifier of the company adding the job.</param>
        /// <param name="skillLinks">The list of skill links associated with the job posting.</param>
        /// <returns>The unique identifier of the newly added job posting.</returns>
        int AddJob(Job Job, int companyId, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks);

        /// <summary>
        /// Retrieves all skills linked to a specific job posting.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting.</param>
        /// <returns>A read-only list of skill links with required percentages.</returns>
        IReadOnlyList<(int SkillId, int RequiredPercentage)> GetSkillsByJob(int jobId);

        /// <summary>
        /// Retrieves a single job posting by its unique identifier.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting.</param>
        /// <returns>The matching <see cref="Job"/>, or <c>null</c> if not found.</returns>
        Job? GetJobById(int jobId);

        /// <summary>
        /// Updates an existing job posting and replaces its associated skill links.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting to update.</param>
        /// <param name="updatedJob">The updated job posting data. Cannot be null.</param>
        /// <param name="skillLinks">The new list of skill links to associate with the job posting.</param>
        /// <returns><c>true</c> if the update succeeded; <c>false</c> if the job was not found.</returns>
        bool UpdateJob(int jobId, Job updatedJob, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks);

        /// <summary>
        /// Returns the number of applicants (Match records) for a job posting.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting.</param>
        /// <returns>The applicant count.</returns>
        int GetApplicantCount(int jobId);

        /// <summary>
        /// Deletes a job posting and its associated skill links. Applicants (Match records) block
        /// deletion unless <paramref name="force"/> is <c>true</c>, in which case they are
        /// cascade-deleted along with the job.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting to delete.</param>
        /// <param name="force">When <c>true</c>, applicants are cascade-deleted with the job.</param>
        /// <returns>The outcome of the delete attempt.</returns>
        JobDeleteResult DeleteJob(int jobId, bool force);
    }
}

