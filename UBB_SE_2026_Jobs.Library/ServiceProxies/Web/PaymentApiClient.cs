using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.DTOs.Web;

namespace UBB_SE_2026_Jobs.Library.ServiceProxies.Web;

public class PaymentApiClient
{
    private readonly HttpClient http;
    private static readonly string ApiPath = "api/payment";

    public PaymentApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<string> ProcessPaymentAsync(
        int jobId,
        int amount,
        string cardHolderName,
        string cardNumber,
        string expDate,
        string cvv)
    {
        var payload = new
        {
            CardHolderName = cardHolderName,
            CardNumber = cardNumber,
            ExpDate = expDate,
            Cvv = cvv
        };

        HttpResponseMessage response =
            await this.http.PostAsJsonAsync(
                $"{ApiPath}/process/{jobId}?paymentAmount={amount}",
                payload);

        if (!response.IsSuccessStatusCode)
            return "Payment failed. Please try again.";

        return string.Empty;
    }

    public async Task<List<JobPaymentInfoDto>> GetPaidJobsInfoAsync(
        string jobType,
        string experienceLevel)
    {
        var response = await this.http.GetAsync(
            $"{ApiPath}/paid?jobType={jobType}&experienceLevel={experienceLevel}");

        if (!response.IsSuccessStatusCode)
            return new List<JobPaymentInfoDto>();

        return await response.Content.ReadFromJsonAsync<List<JobPaymentInfoDto>>()
            ?? new List<JobPaymentInfoDto>();
    }
}
