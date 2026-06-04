using Microsoft.EntityFrameworkCore;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Persistence;

namespace UBB_SE_2026_Jobs.Library.Repositories.Messages;

public class MessageRepository : IMessageRepository
{
    private readonly JobsDbContext databaseContext;

    public MessageRepository(JobsDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<Message>> GetForChatAsync(int chatId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.Messages
            .AsNoTracking()
            .Include(message => message.Chat)
            .Where(message => message.Chat.ChatId == chatId)
            .OrderBy(message => message.Timestamp)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Message?> GetLatestForChatAsync(int chatId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.Messages
            .AsNoTracking()
            .Include(message => message.Chat)
            .Where(message => message.Chat.ChatId == chatId)
            .OrderBy(message => message.Timestamp)
            .LastOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<int> GetUnreadCountAsync(int chatId, int senderId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.Messages
            .CountAsync(
                message => message.Chat.ChatId == chatId
                        && message.Sender.SenderId != senderId
                        && !message.IsRead,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        if (message.Chat is not null)
        {
            databaseContext.Attach(message.Chat);
        }
        databaseContext.Messages.Add(message);
        await databaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return message;
    }

    public async Task MarkAsReadAsync(int chatId, int readerId, CancellationToken cancellationToken = default)
    {
        await databaseContext.Messages
            .Where(message => message.Chat.ChatId == chatId && message.Sender.SenderId != readerId && !message.IsRead)
            .ExecuteUpdateAsync(setters => setters.SetProperty(message => message.IsRead, true), cancellationToken)
            .ConfigureAwait(false);
    }
}

