namespace UBB_SE_2026_Jobs.Library.DTOs;

public class DocumentAddRequest
{
    public int UserId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}
