using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Repositories.Messages;

public interface IMessageRepository
{
    Task<IReadOnlyList<Message>> GetForChatAsync(int chatId, CancellationToken cancellationToken = default);

    Task<Message?> GetLatestForChatAsync(int chatId, CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(int chatId, int senderId, CancellationToken cancellationToken = default);

    Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(int chatId, int readerId, CancellationToken cancellationToken = default);
}
