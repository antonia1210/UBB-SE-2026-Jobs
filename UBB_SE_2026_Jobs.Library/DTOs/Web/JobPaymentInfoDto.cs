namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class JobPaymentInfoDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public int AmountPayed { get; set; }
}
