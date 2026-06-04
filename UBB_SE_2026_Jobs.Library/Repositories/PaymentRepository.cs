namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    public class PaymentRepository : IPaymentRepository
    {
        private readonly JobsDbContext JobsDbContext;

        public PaymentRepository(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc />
        public void UpdateJobPayment(int jobId, int paymentAmount)
        {
            var job = this.JobsDbContext.Jobs.Find(jobId);
            if (job == null)
            {
                throw new System.Exception("Job ID not found. Payment not applied to database.");
            }

            job.AmountPayed = paymentAmount;
            this.JobsDbContext.SaveChanges();
        }

        /// <inheritdoc />
        public List<JobPaymentInfo> GetPaidJobs(string jobType, string experienceLevel)
        {
            return this.JobsDbContext.Jobs
                .Where(j => j.JobType == jobType && j.ExperienceLevel == experienceLevel)
                .Join(
                    this.JobsDbContext.Companies,
                    j => j.CompanyId,
                    c => c.CompanyId,
                    (j, c) => new JobPaymentInfo
                    {
                        CompanyName = c.Name,
                        JobTitle = j.JobTitle,
                        AmountPayed = j.AmountPayed ?? 0,
                    })
                .ToList();
        }

        /// <inheritdoc />
        public List<string> GetCompaniesToNotify(int currentJobId, int newPaymentAmount)
        {
            var currentJob = this.JobsDbContext.Jobs.Find(currentJobId);
            if (currentJob == null)
            {
                return [];
            }

            return this.JobsDbContext.Jobs
                .Where(j => j.JobId != currentJobId
                    && j.JobType == currentJob.JobType
                    && j.ExperienceLevel == currentJob.ExperienceLevel
                    && (j.AmountPayed == null || j.AmountPayed < newPaymentAmount))
                .Join(
                    this.JobsDbContext.Companies.Where(c => c.Email != null && c.Email != string.Empty),
                    j => j.CompanyId,
                    c => c.CompanyId,
                    (j, c) => c.Email)
                .Distinct()
                .ToList();
        }
    }
}

