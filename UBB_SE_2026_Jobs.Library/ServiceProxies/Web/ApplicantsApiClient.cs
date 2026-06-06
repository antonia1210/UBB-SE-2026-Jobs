using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.DTOs.Web;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public class ApplicantsApiClient
{
    private readonly HttpClient httpClient;

    public ApplicantsApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public void SetAuthToken(string jwt)
    {
        this.httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);
    }

    public async Task<ApplicantDto?> CreateApplicantAsync(ApplicantDto dto)
    {
        try
        {
            Debug.WriteLine($"Creating applicant for JobId: {dto.JobId}, UserId: {dto.UserId}");
            Debug.WriteLine($"Authorization header: {this.httpClient.DefaultRequestHeaders.Authorization}");

            HttpResponseMessage response =
                await this.httpClient.PostAsJsonAsync("api/applicants", dto);

            if (response.IsSuccessStatusCode)
            {
                ApplicantDto? result = await response.Content.ReadFromJsonAsync<ApplicantDto>();
                Debug.WriteLine($"Successfully created applicant: {result?.ApplicantId}");
                return result;
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Response Status: {response.StatusCode}");
                Debug.WriteLine($"API Response Content: {content}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception creating applicant: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ApplicantDto>> GetApplicantsByJobAsync(int jobId)
    {
        try
        {
            HttpResponseMessage response =
                await this.httpClient.GetAsync($"api/applicants/byjob/{jobId}");

            if (response.IsSuccessStatusCode)
            {
                List<ApplicantDto>? result =
                    await response.Content.ReadFromJsonAsync<List<ApplicantDto>>();
                return result ?? new List<ApplicantDto>();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Debug.WriteLine($"No applicants found for job {jobId} (404)");
                return new List<ApplicantDto>();
            }
            else
            {
                Debug.WriteLine($"Error fetching applicants for job {jobId}: {response.StatusCode}");
                return new List<ApplicantDto>();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception fetching applicants for job {jobId}: {ex.Message}");
            return new List<ApplicantDto>();
        }
    }

    public async Task<ApplicantDto?> GetApplicantByIdAsync(int applicantId)
    {
        try
        {
            ApplicantDto? result =
                await this.httpClient.GetFromJsonAsync<ApplicantDto>($"api/applicants/{applicantId}");
            return result;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> HasUserAppliedAsync(int jobId, int userId)
    {
        List<ApplicantDto> applicants = await this.GetApplicantsByJobAsync(jobId);
        return applicants.Any(applicant => applicant.UserId == userId);
    }
}
