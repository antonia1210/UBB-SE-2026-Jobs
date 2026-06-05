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

                // Update scalar fields; CompanyId, PostedAt, Photo and JobRole are preserved.
                existing.JobTitle = updatedJob.JobTitle;
                existing.IndustryField = updatedJob.IndustryField;
                existing.JobType = updatedJob.JobType;
                existing.ExperienceLevel = updatedJob.ExperienceLevel;
                existing.JobLocation = updatedJob.JobLocation;
                existing.JobDescription = updatedJob.JobDescription;
                existing.AvailablePositions = updatedJob.AvailablePositions;
                existing.Salary = updatedJob.Salary;
                existing.StartDate = updatedJob.StartDate;
                existing.EndDate = updatedJob.EndDate;
                existing.Deadline = updatedJob.Deadline;
                existing.AmountPayed = updatedJob.AmountPayed ?? existing.AmountPayed;

                // Replace skill links only when the caller actually supplies them; otherwise
                // preserve the existing links (e.g. the web Edit form has no skills editor and
                // would otherwise wipe them on every save).
                if (skillLinks != null && skillLinks.Count > 0)
                {
                    if (existing.JobSkills != null)
                    {
                        this.JobsDbContext.JobSkills.RemoveRange(existing.JobSkills);
                    }

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
        public int GetApplicantCount(int jobId)
        {
            return this.JobsDbContext.Matches
                .Count(match => EF.Property<int>(match, "JobId") == jobId);
        }

        /// <inheritdoc />
        public JobDeleteResult DeleteJob(int jobId, bool force)
        {
            using var transaction = this.JobsDbContext.Database.BeginTransaction();

            try
            {
                Job? job = this.JobsDbContext.Jobs
                    .Include(j => j.JobSkills)
                    .FirstOrDefault(j => j.JobId == jobId);

                if (job == null)
                {
                    return JobDeleteResult.NotFound;
                }

                // Applicants (Match records) are real application history; only remove them when the
                // caller explicitly forces a cascade delete.
                List<Match> applicants = this.JobsDbContext.Matches
                    .Where(match => EF.Property<int>(match, "JobId") == jobId)
                    .ToList();

                if (applicants.Count > 0 && !force)
                {
                    return JobDeleteResult.HasApplicants;
                }

                if (applicants.Count > 0)
                {
                    this.JobsDbContext.Matches.RemoveRange(applicants);
                }

                // Recommendations are derived data; always remove them so the FK does not block.
                List<Recommendation> recommendations = this.JobsDbContext.Recommendations
                    .Where(recommendation => EF.Property<int>(recommendation, "JobId") == jobId)
                    .ToList();
                if (recommendations.Count > 0)
                {
                    this.JobsDbContext.Recommendations.RemoveRange(recommendations);
                }

                // Chats are kept but unlinked from the job (nullable FK).
                List<Chat> linkedChats = this.JobsDbContext.Chats
                    .Where(chat => EF.Property<int?>(chat, "JobId") == jobId)
                    .ToList();
                foreach (Chat chat in linkedChats)
                {
                    chat.Job = null;
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

