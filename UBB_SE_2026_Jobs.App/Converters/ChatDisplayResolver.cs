using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.App.Converters;

internal static class ChatDisplayResolver
{
    private const int MissingSenderId = 0;
    private const string DefaultChatName = "Chat";

    public static string ResolveChatName(Chat chat)
    {
        var session = App.Services.GetService(typeof(SessionContext)) as SessionContext;

        if (chat.SecondUser is not null)
        {
            if (session is not null)
            {
                return chat.User.UserId == session.UserId ? chat.SecondUser.Name : chat.User.Name;
            }
            return chat.SecondUser.Name;
        }

        if (chat.Company is not null)
        {
            return !string.IsNullOrWhiteSpace(chat.Company.Name)
                ? chat.Company.Name
                : $"Company {chat.Company.CompanyId}";
        }

        return DefaultChatName;
    }

    public static int GetCurrentSenderId()
    {
        var session = App.Services.GetService(typeof(SessionContext)) as SessionContext;
        return session?.UserId ?? MissingSenderId;
    }
}
