namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories.Interfaces
{
    using System.Collections.Generic;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models;

    public interface IPaymentRepository
    {
        void UpdateJobPayment(int jobId, int paymentAmount);
        List<JobPaymentInfo> GetPaidJobs(string jobType, string experienceLevel);
        List<string> GetCompaniesToNotify(int currentJobId, int newPaymentAmount);
    }
}