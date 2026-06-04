namespace UBB_SE_2026_Jobs.Library.Repositories.Portal
{
    using System.Collections.Generic;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;

    public interface IPaymentRepository
    {
        void UpdateJobPayment(int jobId, int paymentAmount);
        List<JobPaymentInfo> GetPaidJobs(string jobType, string experienceLevel);
        List<string> GetCompaniesToNotify(int currentJobId, int newPaymentAmount);
    }
}