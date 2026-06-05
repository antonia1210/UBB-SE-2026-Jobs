namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    public class ApplicantRepository : IApplicantRepository
    {
        private readonly JobsDbContext JobsDbContext;

        public ApplicantRepository(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc/>
        public Applicant GetApplicantById(int applicantId)
        {
            return this.JobsDbContext.Applicants
                .Include(a => a.User)
                .Include(a => a.Job)
                .Include(a => a.RecommendedFromCompany)
                .FirstOrDefault(a => a.ApplicantId == applicantId);
        }

        /// <inheritdoc/>
        public IEnumerable<Applicant> GetApplicantsByCompany(int companyId)
        {
            return this.JobsDbContext.Applicants
                .Include(a => a.User)
                .Include(a => a.Job)
                .Include(a => a.RecommendedFromCompany)
                .Where(a => a.Job.CompanyId == companyId)
                .ToList();
        }

        /// <inheritdoc/>
        public IEnumerable<Applicant> GetApplicantsByJob(Job Job)
        {
            if (Job == null)
            {
                return new List<Applicant>();
            }

            return this.JobsDbContext.Applicants
                .Include(a => a.User)
                .Include(a => a.Job)
                .Include(a => a.RecommendedFromCompany)
                .Where(a => a.JobId == Job.JobId)
                .ToList();
        }

        /// <inheritdoc/>
        public Applicant? GetPendingApplicantByJobAndUser(int jobId, int userId)
        {
            return this.JobsDbContext.Applicants
                .FirstOrDefault(a => a.JobId == jobId && a.UserId == userId);
        }

        /// <inheritdoc/>
        public void AddApplicant(Applicant applicant)
        {
            this.JobsDbContext.Applicants.Add(applicant);
            this.JobsDbContext.SaveChanges();
        }

        /// <inheritdoc/>
        public void UpdateApplicant(Applicant applicant)
        {
            var existing = this.JobsDbContext.Applicants.Find(applicant.ApplicantId);
            if (existing == null)
            {
                return;
            }

            existing.AppTestGrade = applicant.AppTestGrade;
            existing.CvGrade = applicant.CvGrade;
            existing.CompanyTestGrade = applicant.CompanyTestGrade;
            existing.InterviewGrade = applicant.InterviewGrade;
            existing.ApplicationStatus = applicant.ApplicationStatus;

            this.JobsDbContext.SaveChanges();
        }

        /// <inheritdoc/>
        public void RemoveApplicant(int applicantId)
        {
            var applicant = this.JobsDbContext.Applicants.Find(applicantId);
            if (applicant != null)
            {
                this.JobsDbContext.Applicants.Remove(applicant);
                this.JobsDbContext.SaveChanges();
            }
        }
    }
}

