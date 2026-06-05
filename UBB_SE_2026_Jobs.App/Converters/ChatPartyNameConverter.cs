using Microsoft.UI.Xaml.Data;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.App.Converters;

public class ChatPartyNameConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        return value is Chat chat
            ? ChatDisplayResolver.ResolveChatName(chat)
            : "No chat selected";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}
