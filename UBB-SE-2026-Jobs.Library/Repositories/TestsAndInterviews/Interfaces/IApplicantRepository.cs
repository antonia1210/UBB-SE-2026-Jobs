namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories.Interfaces
{
    using System.Collections.Generic;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models;

    public interface IApplicantRepository
    {
        Applicant GetApplicantById(int applicantId);
        public IEnumerable<Applicant> GetApplicantsByCompany(int companyId);
        IEnumerable<Applicant> GetApplicantsByJob(JobPosting job);
        void AddApplicant(Applicant applicant);
        void UpdateApplicant(Applicant applicant);
        void RemoveApplicant(int applicantId);
    }
}
