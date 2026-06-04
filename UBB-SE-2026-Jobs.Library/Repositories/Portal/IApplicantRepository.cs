namespace UBB_SE_2026_Jobs.Library.Repositories.Portal
{
    using System.Collections.Generic;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;

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
