namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? CvXml { get; set; }
}
