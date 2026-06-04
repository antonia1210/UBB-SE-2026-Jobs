using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.PdfExport;

public interface IPdfExportService
{
    Task<string> RenderHtmlAsync(User user);
    Task<byte[]> GeneratePdfAsync(User user);
}
