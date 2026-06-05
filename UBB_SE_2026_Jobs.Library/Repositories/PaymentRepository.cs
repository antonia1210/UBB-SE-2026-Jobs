namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    public class PaymentRepository : IPaymentRepository
    {
        private readonly JobsDbContext databaseContext;

        public PaymentRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <inheritdoc />
        public void UpdateJobPayment(int jobId, int paymentAmount)
        {
            var job = this.databaseContext.Jobs.Find(jobId);
            if (job == null)
            {
                throw new System.Exception("Job ID not found. Payment not applied to database.");
            }

            job.AmountPayed = paymentAmount;
            this.databaseContext.SaveChanges();
        }

        /// <inheritdoc />
        public List<JobPaymentInfo> GetPaidJobs(string jobType, string experienceLevel)
        {
            return this.databaseContext.Jobs
                .Where(job => job.JobType == jobType && job.ExperienceLevel == experienceLevel)
                .Join(
                    this.databaseContext.Companies,
                    job => job.CompanyId,
                    company => company.CompanyId,
                    (job, company) => new JobPaymentInfo
                    {
                        CompanyName = company.Name,
                        JobTitle = job.JobTitle,
                        AmountPayed = job.AmountPayed ?? 0,
                    })
                .ToList();
        }

        /// <inheritdoc />
        public List<string> GetCompaniesToNotify(int currentJobId, int newPaymentAmount)
        {
            var currentJob = this.databaseContext.Jobs.Find(currentJobId);
            if (currentJob == null)
            {
                return [];
            }

            return this.databaseContext.Jobs
                .Where(job => job.JobId != currentJobId
                    && job.JobType == currentJob.JobType
                    && job.ExperienceLevel == currentJob.ExperienceLevel
                    && (job.AmountPayed == null || job.AmountPayed < newPaymentAmount))
                .Join(
                    this.databaseContext.Companies.Where(company => company.Email != null && company.Email != string.Empty),
                    job => job.CompanyId,
                    company => company.CompanyId,
                    (job, company) => company.Email)
                .Distinct()
                .ToList();
        }
    }
}