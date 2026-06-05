namespace UBB_SE_2026_Jobs.Library.Repositories.Interfaces
{
    using System.Collections.Generic;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Services;

    public interface ITestsJobsRepository
    {
        IEnumerable<Job> GetAllJobs();

        /// <summary>All skills in the catalog (for job creation checkboxes).</summary>
        IReadOnlyList<Skill> GetAllSkills();

        /// <summary>Skills linked to a job with required percentages.</summary>
        // IReadOnlyList<JobSkill> GetSkillsForJob(int jobId);

        /// <summary>Inserts a job (new job_id = MAX(job_id)+1) and optional job_skills rows. Returns the new id.</summary>
        int AddJob(Job job, int companyId, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks);

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
        /// Deletes a job posting and its associated skill links. Recommendations are removed and
        /// any chats are unlinked from the job so foreign-key constraints are satisfied. Applicants
        /// (Match records) block deletion unless <paramref name="force"/> is <c>true</c>, in which
        /// case they are cascade-deleted along with the job.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting to delete.</param>
        /// <param name="force">When <c>true</c>, applicants are cascade-deleted with the job.</param>
        /// <returns>The outcome of the delete attempt.</returns>
        JobDeleteResult DeleteJob(int jobId, bool force);
    }
}

