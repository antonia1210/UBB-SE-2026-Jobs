namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class ApplicantDto
{
    public int ApplicantId { get; set; }
    public int JobId { get; set; }
    public int UserId { get; set; }
    public decimal? AppTestGrade { get; set; }
    public decimal? CvGrade { get; set; }
    public decimal? CompanyTestGrade { get; set; }
    public decimal? InterviewGrade { get; set; }
    public string? ApplicationStatus { get; set; }
    public DateTime AppliedAt { get; set; }
    public int? RecommendedFromCompanyId { get; set; }
    public string? CvFileUrl { get; set; }
}
