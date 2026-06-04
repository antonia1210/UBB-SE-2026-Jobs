using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.Documents;

public interface ILocalDocumentFileService
{
    Task<Document> UploadDocumentAsync(Document document, string filePath, CancellationToken cancellationToken = default);

    Task DeleteDocumentAsync(int documentId, CancellationToken cancellationToken = default);

    Task<string> GetDocumentUrlAsync(int documentId, CancellationToken cancellationToken = default);
}
