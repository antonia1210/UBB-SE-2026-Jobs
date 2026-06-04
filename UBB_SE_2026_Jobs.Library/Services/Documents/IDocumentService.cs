using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.Documents;

public interface IDocumentService
{
    Task<IReadOnlyList<Document>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Document?> GetByIdAsync(int documentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Document>> GetDocumentsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Document> AddAsync(Document document, CancellationToken cancellationToken = default);

    Task UpdateAsync(Document document, CancellationToken cancellationToken = default);

    Task RemoveAsync(int documentId, CancellationToken cancellationToken = default);

    Task<Document> UploadDocumentAsync(Document document, string filePath, CancellationToken cancellationToken = default);

    Task DeleteDocumentAsync(int documentId, CancellationToken cancellationToken = default);

    Task<Document> UploadDocumentFromStreamAsync(
        int userId,
        string documentName,
        string originalFileName,
        string contentType,
        Stream fileStream,
        bool isCv,
        CancellationToken cancellationToken = default);

}
