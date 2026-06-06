using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.DTOs.Web;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public class JobsApiClient
{
    private readonly HttpClient httpClient;

    public JobsApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public void SetAuthToken(string jwt)
    {
        this.httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);
    }

    public async Task<List<JobPostingDto>> GetAllJobsAsync()
    {
        List<JobPostingDto>? result =
            await this.httpClient.GetFromJsonAsync<List<JobPostingDto>>("api/TestsJobs");
        return result ?? new List<JobPostingDto>();
    }

    public async Task<List<JobPostingDto>> GetJobsByCompanyIdAsync(int companyId)
    {
        List<JobPostingDto>? result =
            await this.httpClient.GetFromJsonAsync<List<JobPostingDto>>($"api/TestsJobs?companyId={companyId}");
        return result ?? new List<JobPostingDto>();
    }

    public async Task<JobPostingDto?> GetJobByIdAsync(int jobId)
    {
        List<JobPostingDto> jobs = await this.GetAllJobsAsync();
        return jobs.Find(job => job.JobId == jobId);
    }

    public async Task<List<SkillDto>> GetAllSkillsAsync()
    {
        List<SkillDto>? result =
            await this.httpClient.GetFromJsonAsync<List<SkillDto>>("api/TestsJobs/skills");
        return result ?? new List<SkillDto>();
    }

    public async Task<bool> AddJobAsync(AddJobDto dto)
    {
        Debug.WriteLine(dto);
        Debug.WriteLine($"Authorization header: {this.httpClient.DefaultRequestHeaders.Authorization}");
        HttpResponseMessage response =
            await this.httpClient.PostAsJsonAsync("api/TestsJobs", dto);

        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"API Response Status: {response.StatusCode}");
            Debug.WriteLine($"API Response Content: {content}");
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateJobAsync(int jobId, JobPostingDto dto)
    {
        HttpResponseMessage response =
            await this.httpClient.PutAsJsonAsync($"api/TestsJobs/{jobId}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<int> GetApplicantCountAsync(int jobId)
    {
        return await this.httpClient.GetFromJsonAsync<int>(
            $"api/TestsJobs/{jobId}/applicant-count");
    }

    public async Task<JobDeleteResult> DeleteJobAsync(int jobId, bool force)
    {
        string url = $"api/TestsJobs/{jobId}";
        if (force)
        {
            url += "?force=true";
        }

        HttpResponseMessage response = await this.httpClient.DeleteAsync(url);

        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound => JobDeleteResult.NotFound,
            System.Net.HttpStatusCode.Conflict => JobDeleteResult.HasApplicants,
            _ when response.IsSuccessStatusCode => JobDeleteResult.Deleted,
            _ => JobDeleteResult.NotFound,
        };
    }
}
