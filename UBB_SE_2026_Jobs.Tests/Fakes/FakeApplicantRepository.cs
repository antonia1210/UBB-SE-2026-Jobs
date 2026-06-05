using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

namespace UBB_SE_2026_Jobs.Tests.Fakes;

public class FakeApplicantRepository : IApplicantRepository
{
    private readonly List<Applicant> _store = new();

    public void Seed(params Applicant[] applicants)
    {
        _store.AddRange(applicants);
    }

    public Applicant? GetPendingApplicantByJobAndUser(int jobId, int userId)
    {
        return _store.FirstOrDefault(applicant =>
            applicant.JobId == jobId && applicant.UserId == userId);
    }

    public void UpdateApplicant(Applicant applicant)
    {
        var index = _store.FindIndex(stored => stored.ApplicantId == applicant.ApplicantId);
        if (index >= 0)
            _store[index] = applicant;
    }

    public Applicant GetApplicantById(int applicantId)
        => _store.First(applicant => applicant.ApplicantId == applicantId);

    public IEnumerable<Applicant> GetApplicantsByCompany(int companyId)
        => _store.Where(applicant => applicant.Job?.CompanyId == companyId).ToList();

    public IEnumerable<Applicant> GetApplicantsByJob(Job job)
        => _store.Where(applicant => applicant.JobId == job.JobId).ToList();

    public void AddApplicant(Applicant applicant) => _store.Add(applicant);

    public void RemoveApplicant(int applicantId)
        => _store.RemoveAll(applicant => applicant.ApplicantId == applicantId);
}
