using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.Jobs;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies;

public class JobServiceProxy : IJobService
{
    private readonly HttpClient http;

    public JobServiceProxy(HttpClient http)
    {
        this.http = http;
    }

    public async Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<Job>>("api/jobs", cancellationToken) ?? new List<Job>();

    public async Task<Job?> GetByIdAsync(int jobId, CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync($"api/jobs/{jobId}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Job>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<Job>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<List<Job>>($"api/jobs?companyId={companyId}", cancellationToken) ?? new List<Job>();

    public async Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("api/jobs", job, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Job>(cancellationToken: cancellationToken))!;
    }

    public async Task UpdateAsync(Job job, CancellationToken cancellationToken = default)
    {
        var response = await http.PutAsJsonAsync($"api/jobs/{job.JobId}", job, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<int> GetApplicantCountAsync(int jobId, CancellationToken cancellationToken = default)
        => await http.GetFromJsonAsync<int>($"api/jobs/{jobId}/applicant-count", cancellationToken);

    public async Task<JobDeleteResult> RemoveAsync(int jobId, bool force, CancellationToken cancellationToken = default)
    {
        var url = force ? $"api/jobs/{jobId}?force=true" : $"api/jobs/{jobId}";
        var response = await http.DeleteAsync(url, cancellationToken);

        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound => JobDeleteResult.NotFound,
            System.Net.HttpStatusCode.Conflict => JobDeleteResult.HasApplicants,
            _ when response.IsSuccessStatusCode => JobDeleteResult.Deleted,
            _ => JobDeleteResult.NotFound,
        };
    }
}
