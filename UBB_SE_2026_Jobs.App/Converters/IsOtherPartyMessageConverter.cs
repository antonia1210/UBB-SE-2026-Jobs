using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.App.Converters;

public class IsOtherPartyMessageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is not Message message)
        {
            return Visibility.Collapsed;
        }

        var currentSenderId = ChatDisplayResolver.GetCurrentSenderId();
        return message.Sender.SenderId != currentSenderId ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}
