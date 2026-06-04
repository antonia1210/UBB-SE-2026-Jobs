namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    public class TestsJobsRepository : ITestsJobsRepository
    {
        private const int MinimumSkillPercentage = 1;
        private const int MaximumSkillPercentage = 100;

        private readonly JobsDbContext JobsDbContext;

        public TestsJobsRepository(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc />
        public IEnumerable<Job> GetAllJobs()
        {
            return this.JobsDbContext.Jobs
                .Include(j => j.Company)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .ToList();
        }

        /// <inheritdoc />
        public IReadOnlyList<Skill> GetAllSkills()
        {
            return this.JobsDbContext.Skills.ToList();
        }

        /// <inheritdoc />
        public Job? GetJobById(int jobId)
        {
            return this.JobsDbContext.Jobs
                .Include(j => j.Company)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .FirstOrDefault(j => j.JobId == jobId);
        }

        /// <inheritdoc />
        public int AddJob(Job Job, int companyId, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks)
        {
            if (Job == null)
            {
                throw new ArgumentNullException(nameof(Job));
            }

            using var transaction = this.JobsDbContext.Database.BeginTransaction();

            try
            {
                Job.CompanyId = companyId;
                Job.AmountPayed ??= 0;
                Job.PostedAt ??= DateTime.Now;

                this.JobsDbContext.Jobs.Add(Job);
                this.JobsDbContext.SaveChanges();

                if (skillLinks != null)
                {
                    foreach (var (skillId, percentage) in skillLinks)
                    {
                        if (percentage < MinimumSkillPercentage || percentage > MaximumSkillPercentage)
                        {
                            continue;
                        }

                        this.JobsDbContext.JobSkills.Add(new JobSkill
                        {
                            JobId = Job.JobId,
                            SkillId = skillId,
                            RequiredPercentage = percentage,
                        });
                    }

                    this.JobsDbContext.SaveChanges();
                }

                var company = this.JobsDbContext.Companies.Find(companyId);
                if (company != null)
                {
                    company.PostedJobsCount += 1;
                    this.JobsDbContext.SaveChanges();
                }

                transaction.Commit();
                return Job.JobId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <inheritdoc />
        public bool UpdateJob(int jobId, Job updatedJob, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks)
        {
            if (updatedJob == null)
            {
                throw new ArgumentNullException(nameof(updatedJob));
            }

            using var transaction = this.JobsDbContext.Database.BeginTransaction();

            try
            {
                Job? existing = this.JobsDbContext.Jobs
                    .Include(j => j.JobSkills)
                    .FirstOrDefault(j => j.JobId == jobId);

                if (existing == null)
                {
                    return false;
                }

                // Update scalar fields only; CompanyId and PostedAt are preserved
                existing.JobTitle = updatedJob.JobTitle;
                existing.JobDescription = updatedJob.JobDescription;
                existing.AmountPayed = updatedJob.AmountPayed ?? existing.AmountPayed;

                // Replace skill links: remove old ones, insert new ones
                if (existing.JobSkills != null)
                {
                    this.JobsDbContext.JobSkills.RemoveRange(existing.JobSkills);
                }

                if (skillLinks != null)
                {
                    foreach (var (skillId, percentage) in skillLinks)
                    {
                        if (percentage < MinimumSkillPercentage || percentage > MaximumSkillPercentage)
                        {
                            continue;
                        }

                        this.JobsDbContext.JobSkills.Add(new JobSkill
                        {
                            JobId = jobId,
                            SkillId = skillId,
                            RequiredPercentage = percentage,
                        });
                    }
                }

                this.JobsDbContext.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <inheritdoc />
        public bool DeleteJob(int jobId)
        {
            using var transaction = this.JobsDbContext.Database.BeginTransaction();

            try
            {
                Job? job = this.JobsDbContext.Jobs
                    .Include(j => j.JobSkills)
                    .FirstOrDefault(j => j.JobId == jobId);

                if (job == null)
                {
                    return false;
                }

                // Remove skill links first to respect FK constraints
                if (job.JobSkills != null)
                {
                    this.JobsDbContext.JobSkills.RemoveRange(job.JobSkills);
                }

                this.JobsDbContext.Jobs.Remove(job);

                // Decrement the company's posted job count
                var company = this.JobsDbContext.Companies.Find(job.CompanyId);
                if (company != null && company.PostedJobsCount > 0)
                {
                    company.PostedJobsCount -= 1;
                }

                this.JobsDbContext.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

