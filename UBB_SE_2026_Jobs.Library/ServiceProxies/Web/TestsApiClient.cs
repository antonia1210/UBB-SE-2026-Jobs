using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.DTOs.Web;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public class TestsApiClient
{
    private readonly HttpClient http;
    private static readonly string ApiPath = "api/tests";
    private static readonly string AttemptsApiPath = "api/testattempts";
    private static readonly string QuestionsApiPath = "api/questions";

    public TestsApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<TestDto>> GetAll()
    {
        return await this.http.GetFromJsonAsync<List<TestDto>>(ApiPath) ?? new List<TestDto>();
    }

    public async Task<List<string>> GetCategories()
    {
        return await this.http.GetFromJsonAsync<List<string>>($"{ApiPath}/categories") ?? new List<string>();
    }

    public async Task<List<TestDto>> GetByCategory(string category)
    {
        string encodedCategory = Uri.EscapeDataString(category);
        return await this.http.GetFromJsonAsync<List<TestDto>>($"{ApiPath}/bycategory/{encodedCategory}") ?? new List<TestDto>();
    }

    public async Task<TestDto?> GetById(int id)
    {
        return await this.http.GetFromJsonAsync<TestDto>($"{ApiPath}/{id}");
    }

    public async Task<float> SubmitAttemptAsync(int userId, int testId, IEnumerable<AnswerDto> answers)
    {
        var payload = new
        {
            UserId = userId,
            TestId = testId,
            Answers = answers
        };

        var response = await this.http.PostAsJsonAsync($"{ApiPath}/submit-attempt", payload);

        if (!response.IsSuccessStatusCode)
            return 0f;

        return await response.Content.ReadFromJsonAsync<float>();
    }

    public async Task<List<QuestionDto>> GetQuestionsByTestIdAsync(int testId)
    {
        var response = await this.http.GetAsync($"{QuestionsApiPath}/bytest/{testId}");

        if (!response.IsSuccessStatusCode)
            return new List<QuestionDto>();

        return await response.Content.ReadFromJsonAsync<List<QuestionDto>>() ?? new List<QuestionDto>();
    }

    public async Task<bool> AttemptExistsAsync(int userId, int testId)
    {
        var response = await this.http.GetAsync($"{AttemptsApiPath}/byuser/{userId}/bytest/{testId}");
        return response.IsSuccessStatusCode;
    }

    public async Task StartAttemptAsync(int userId, int testId)
    {
        var payload = new
        {
            UserId = userId,
            ExternalUserId = userId,
            TestId = testId,
            Status = "IN_PROGRESS",
            StartedAt = DateTime.UtcNow
        };

        await this.http.PostAsJsonAsync(AttemptsApiPath, payload);
    }

    public async Task<TestAttemptDto?> GetAttemptByUserAndTestAsync(int userId, int testId)
    {
        var response = await this.http.GetAsync($"{AttemptsApiPath}/byuser/{userId}/bytest/{testId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<TestAttemptDto>();
    }

    public async Task<TestAttemptDto?> UpdateAttemptAsync(int attemptId, TestAttemptDto attemptDto)
    {
        var response = await this.http.PutAsJsonAsync($"{AttemptsApiPath}/{attemptId}", attemptDto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<TestAttemptDto>();
    }

    public async Task<AnswerDto> GradeAnswerAsync(string questionType, object gradeRequestPayload)
    {
        string endpoint = questionType switch
        {
            "SINGLE_CHOICE" => "single-choice",
            "MULTIPLE_CHOICE" => "multiple-choice",
            "TRUE_FALSE" => "true-false",
            "TEXT" => "text",
            _ => "text"
        };

        var response = await this.http.PostAsJsonAsync($"api/grading/{endpoint}", gradeRequestPayload);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AnswerDto>() ?? new AnswerDto();
    }

    public async Task SaveAnswerAsync(AnswerDto dto)
    {
        var response = await this.http.PostAsJsonAsync("api/answers", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task<float> CalculateFinalScoreAsync(object attemptPayload)
    {
        var response = await this.http.PostAsJsonAsync("api/grading/final-score", attemptPayload);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<float>();
    }

    public async Task<List<TestAttemptDto>> GetValidAttemptsByTestIdAsync(int testId)
    {
        var response = await this.http.GetAsync($"{AttemptsApiPath}/valid/bytest/{testId}");

        if (!response.IsSuccessStatusCode)
            return new List<TestAttemptDto>();

        return await response.Content.ReadFromJsonAsync<List<TestAttemptDto>>() ?? new List<TestAttemptDto>();
    }

    public async Task<List<TestAttemptDto>> GetAttemptsByUserAsync(int userId)
    {
        var response = await this.http.GetAsync($"{AttemptsApiPath}/byuser/{userId}");

        if (!response.IsSuccessStatusCode)
            return new List<TestAttemptDto>();

        return await response.Content.ReadFromJsonAsync<List<TestAttemptDto>>() ?? new List<TestAttemptDto>();
    }

    public async Task<List<AnswerDto>> GetAnswersByAttemptIdAsync(int attemptId)
    {
        var response = await this.http.GetAsync($"api/answers/byattempt/{attemptId}");

        if (!response.IsSuccessStatusCode)
            return new List<AnswerDto>();

        return await response.Content.ReadFromJsonAsync<List<AnswerDto>>() ?? new List<AnswerDto>();
    }
}
