using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.DTOs.Web;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public class QuestionsApiClient
{
    private readonly HttpClient _http;
    private static string s_apiPath = "/api/questions";

    public QuestionsApiClient(HttpClient http)
    {
        this._http = http;
    }

    public async Task<List<QuestionDto>> GetQuestions(int? testId = null)
    {
        if (testId.HasValue)
        {
            return await this._http.GetFromJsonAsync<List<QuestionDto>>($"{s_apiPath}/bytest/{testId.Value}") ?? new List<QuestionDto>();
        }

        return await this._http.GetFromJsonAsync<List<QuestionDto>>(s_apiPath) ?? new List<QuestionDto>();
    }

    public async Task<QuestionDto?> GetQuestion(int id)
    {
        return await this._http.GetFromJsonAsync<QuestionDto>($"{s_apiPath}/{id}");
    }

    public async Task<List<QuestionDto>> GetByTest(int testId)
    {
        var response = await this._http.GetAsync($"{s_apiPath}/bytest/{testId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return new List<QuestionDto>();

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<QuestionDto>>() ?? new List<QuestionDto>();
    }
}
