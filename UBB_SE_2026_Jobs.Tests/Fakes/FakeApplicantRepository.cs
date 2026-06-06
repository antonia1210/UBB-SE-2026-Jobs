using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

namespace UBB_SE_2026_Jobs.Tests.Fakes;

public class FakeApplicantRepository : IApplicantRepository
{
    private readonly List<Applicant> applicants = new();

    public void Seed(params Applicant[] applicants)
    {
        this.applicants.AddRange(applicants);
    }

    public Applicant? GetPendingApplicantByJobAndUser(int jobId, int userId)
    {
        return applicants.FirstOrDefault(applicant =>
            applicant.JobId == jobId && applicant.UserId == userId);
    }

    public void UpdateApplicant(Applicant applicant)
    {
        var index = applicants.FindIndex(stored => stored.ApplicantId == applicant.ApplicantId);
        if (index >= 0)
            applicants[index] = applicant;
    }

    public Applicant GetApplicantById(int applicantId)
        => applicants.First(applicant => applicant.ApplicantId == applicantId);

    public IEnumerable<Applicant> GetApplicantsByCompany(int companyId)
        => applicants.Where(applicant => applicant.Job?.CompanyId == companyId).ToList();

    public IEnumerable<Applicant> GetApplicantsByJob(Job job)
        => applicants.Where(applicant => applicant.JobId == job.JobId).ToList();

    public void AddApplicant(Applicant applicant) => applicants.Add(applicant);

    public void RemoveApplicant(int applicantId)
        => applicants.RemoveAll(applicant => applicant.ApplicantId == applicantId);
}
