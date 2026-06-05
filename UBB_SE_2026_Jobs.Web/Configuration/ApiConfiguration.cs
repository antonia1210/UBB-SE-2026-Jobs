namespace UBB_SE_2026_Jobs.Web.Configuration;

public record ApiConfiguration(string BaseUrl)
{
    // TODO: remove when auth is implemented
    public int TemporaryCompanyId { get; init; } = 1;
}