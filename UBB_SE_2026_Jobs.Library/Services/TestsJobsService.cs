namespace UBB_SE_2026_Jobs.Library.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing job postings.
    /// </summary>
    public class TestsJobsService : ITestsJobsService
    {
        private readonly ITestsJobsRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestsJobsService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access job posting data. Cannot be null.</param>
        public TestsJobsService(ITestsJobsRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Retrieves all job postings.
        /// </summary>
        /// <returns>A list of all job postings.</returns>
        public IEnumerable<Job> GetAllJobs()
        {
            return this._repository.GetAllJobs();
        }

        /// <summary>
        /// Retrieves all available skills.
        /// </summary>
        /// <returns>A read-only list of all skills.</returns>
        public IReadOnlyList<Skill> GetAllSkills()
        {
            return this._repository.GetAllSkills();
        }

        /// <summary>
        /// Retrieves a single job posting by its unique identifier.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting.</param>
        /// <returns>The matching <see cref="Job"/>, or <c>null</c> if not found.</returns>
        public Job? GetJobById(int jobId)
        {
            return this._repository.GetJobById(jobId);
        }

        /// <summary>
        /// Adds a new job posting to the data store.
        /// </summary>
        /// <param name="Job">The job posting to add. Cannot be null.</param>
        /// <param name="companyId">The unique identifier of the company adding the job.</param>
        /// <param name="skillLinks">The list of skill links associated with the job posting.</param>
        /// <returns>The unique identifier of the newly added job posting.</returns>
        public int AddJob(Job Job, int companyId, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks)
        {
            return this._repository.AddJob(Job, companyId, skillLinks);
        }

        /// <summary>
        /// Updates an existing job posting and replaces its associated skill links.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting to update.</param>
        /// <param name="updatedJob">The updated job posting data. Cannot be null.</param>
        /// <param name="skillLinks">The new list of skill links to associate with the job posting.</param>
        /// <returns><c>true</c> if the update succeeded; <c>false</c> if the job was not found.</returns>
        public bool UpdateJob(int jobId, Job updatedJob, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks)
        {
            return this._repository.UpdateJob(jobId, updatedJob, skillLinks);
        }

        /// <summary>
        /// Deletes a job posting and all its associated skill links.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting to delete.</param>
        /// <returns><c>true</c> if the deletion succeeded; <c>false</c> if the job was not found.</returns>
        public bool DeleteJob(int jobId)
        {
            return this._repository.DeleteJob(jobId);
        }

        /// <summary>
        /// Retrieves all skills linked to a specific job posting.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting.</param>
        /// <returns>A read-only list of skill links with required percentages.</returns>
        public IReadOnlyList<(int SkillId, int RequiredPercentage)> GetSkillsByJob(int jobId)
        {
            Job? job = this._repository.GetJobById(jobId);
            if (job == null || job.JobSkills == null)
            {
                return new List<(int, int)>();
            }

            return job.JobSkills
                .Select(job => (job.SkillId, job.RequiredPercentage))
                .ToList();
        }
    }
}

