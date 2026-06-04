using System.Text.Json.Serialization;

namespace UBB_SE_2026_Jobs.Library.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobRole
{
    FrontendDeveloper,
    BackendDeveloper,
    UiUxDesigner,
    DevOpsEngineer,
    ProjectManager,
    DataAnalyst,
    CybersecuritySpecialist,
    AiMlEngineer,
}
