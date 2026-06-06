namespace UBB_SE_2026_Jobs.Library.DTOs.TI;

public class TiEventDto
{
    public int Id { get; set; }
    public string Photo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int HostCompanyId { get; set; }
    public DateTime PostedAt { get; set; }
    public List<int> CollaboratorCompanyIds { get; set; } = new();
}
