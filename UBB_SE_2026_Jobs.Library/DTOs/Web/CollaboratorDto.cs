using System.Text.Json.Serialization;

namespace UBB_SE_2026_Jobs.Library.DTOs.Web;

public class CollaboratorDto
{
    public int CompanyId { get; set; }

    [JsonPropertyName("Name")]
    public string CompanyName { get; set; } = string.Empty;
}
