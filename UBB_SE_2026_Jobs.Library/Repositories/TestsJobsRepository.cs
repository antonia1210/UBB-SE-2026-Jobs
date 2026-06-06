namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
    using UBB_SE_2026_Jobs.Library.Services;

    public class TestsJobsRepository : ITestsJobsRepository
    {
        private const int MinimumSkillPercentage = 1;
        private const int MaximumSkillPercentage = 100;

        private readonly JobsDbContext databaseContext;

        public TestsJobsRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <inheritdoc />
        public IEnumerable<Job> GetAllJobs()
        {
            return this.databaseContext.Jobs
                .Include(job => job.Company)
                .Include(job => job.JobSkills)
                    .ThenInclude(jobSkill => jobSkill.Skill)
                .ToList();
        }

        /// <inheritdoc />
        public IReadOnlyList<Skill> GetAllSkills()
        {
            return this.databaseContext.Skills.ToList();
        }

        /// <inheritdoc />
        public Job? GetJobById(int jobId)
        {
            return this.databaseContext.Jobs
                .Include(job => job.Company)
                .Include(job => job.JobSkills)
                    .ThenInclude(jobSkill => jobSkill.Skill)
                .FirstOrDefault(job => job.JobId == jobId);
        }

        /// <inheritdoc />
        public int AddJob(Job jobToAdd, int companyId, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks)
        {
            if (jobToAdd == null)
            {
                throw new ArgumentNullException(nameof(jobToAdd));
            }

            using var transaction = this.databaseContext.Database.BeginTransaction();

            try
            {
                jobToAdd.CompanyId = companyId;
                jobToAdd.AmountPayed ??= 0;
                jobToAdd.PostedAt ??= DateTime.Now;

                this.databaseContext.Jobs.Add(jobToAdd);
                this.databaseContext.SaveChanges();

                if (skillLinks != null)
                {
                    foreach (var (skillId, percentage) in skillLinks)
                    {
                        if (percentage < MinimumSkillPercentage || percentage > MaximumSkillPercentage)
                        {
                            continue;
                        }

                        this.databaseContext.JobSkills.Add(new JobSkill
                        {
                            JobId = jobToAdd.JobId,
                            SkillId = skillId,
                            RequiredPercentage = percentage,
                        });
                    }

                    this.databaseContext.SaveChanges();
                }

                var company = this.databaseContext.Companies.Find(companyId);
                if (company != null)
                {
                    company.PostedJobsCount += 1;
                    this.databaseContext.SaveChanges();
                }

                transaction.Commit();
                return jobToAdd.JobId;
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

            using var transaction = this.databaseContext.Database.BeginTransaction();

            try
            {
                Job? existingJob = this.databaseContext.Jobs
                    .Include(job => job.JobSkills)
                    .FirstOrDefault(job => job.JobId == jobId);

                if (existingJob == null)
                {
                    return false;
                }

                // Update scalar fields; CompanyId, PostedAt, Photo and JobRole are preserved.
                existingJob.JobTitle = updatedJob.JobTitle;
                existingJob.IndustryField = updatedJob.IndustryField;
                existingJob.JobType = updatedJob.JobType;
                existingJob.ExperienceLevel = updatedJob.ExperienceLevel;
                existingJob.JobLocation = updatedJob.JobLocation;
                existingJob.JobDescription = updatedJob.JobDescription;
                existingJob.AvailablePositions = updatedJob.AvailablePositions;
                existingJob.Salary = updatedJob.Salary;
                existingJob.StartDate = updatedJob.StartDate;
                existingJob.EndDate = updatedJob.EndDate;
                existingJob.Deadline = updatedJob.Deadline;
                existingJob.AmountPayed = updatedJob.AmountPayed ?? existingJob.AmountPayed;

                // Replace skill links only when the caller actually supplies them; otherwise
                // preserve the existing links (e.g. the web Edit form has no skills editor and
                // would otherwise wipe them on every save).
                if (skillLinks != null && skillLinks.Count > 0)
                {
                    if (existingJob.JobSkills != null)
                    {
                        this.databaseContext.JobSkills.RemoveRange(existingJob.JobSkills);
                    }

                    foreach (var (skillId, percentage) in skillLinks)
                    {
                        if (percentage < MinimumSkillPercentage || percentage > MaximumSkillPercentage)
                        {
                            continue;
                        }

                        this.databaseContext.JobSkills.Add(new JobSkill
                        {
                            JobId = jobId,
                            SkillId = skillId,
                            RequiredPercentage = percentage,
                        });
                    }
                }

                this.databaseContext.SaveChanges();
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
        public int GetApplicantCount(int jobId)
        {
            return this.databaseContext.Matches
                .Count(match => EF.Property<int>(match, "JobId") == jobId);
        }

        /// <inheritdoc />
        public JobDeleteResult DeleteJob(int jobId, bool force)
        {
            using var transaction = this.databaseContext.Database.BeginTransaction();

            try
            {
                Job? job = this.databaseContext.Jobs
                    .Include(job => job.JobSkills)
                    .FirstOrDefault(job => job.JobId == jobId);

                if (job == null)
                {
                    return JobDeleteResult.NotFound;
                }

                var applicantCount = this.databaseContext.Matches
                    .Count(match => EF.Property<int>(match, "JobId") == jobId);
                if (applicantCount > 0 && !force)
                {
                    return JobDeleteResult.HasApplicants;
                }

                var recommendations = this.databaseContext.Recommendations
                    .Where(recommendation => EF.Property<int>(recommendation, "JobId") == jobId);
                this.databaseContext.Recommendations.RemoveRange(recommendations);

                if (applicantCount > 0)
                {
                    var matches = this.databaseContext.Matches
                        .Where(match => EF.Property<int>(match, "JobId") == jobId);
                    this.databaseContext.Matches.RemoveRange(matches);

                    var applicants = this.databaseContext.Applicants
                        .Where(applicant => applicant.JobId == jobId);
                    this.databaseContext.Applicants.RemoveRange(applicants);
                }

                if (job.JobSkills != null)
                {
                    this.databaseContext.JobSkills.RemoveRange(job.JobSkills);
                }

                this.databaseContext.Jobs.Remove(job);

                var company = this.databaseContext.Companies.Find(job.CompanyId);
                if (company != null && company.PostedJobsCount > 0)
                {
                    company.PostedJobsCount -= 1;
                }

                this.databaseContext.SaveChanges();
                transaction.Commit();
                return JobDeleteResult.Deleted;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
