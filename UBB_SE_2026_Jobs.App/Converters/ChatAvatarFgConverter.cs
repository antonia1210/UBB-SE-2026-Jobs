using Microsoft.UI.Xaml.Data;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.App.Converters;

public class ChatAvatarFgConverter : IValueConverter
{
    private const string DefaultAvatarForegroundColor = "#FF0F4FAD";
    private const string CompanyAvatarForegroundColor = "#FF374151";

    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is not Chat chat)
        {
            return DefaultAvatarForegroundColor;
        }

        return chat.Company!=null ? CompanyAvatarForegroundColor : DefaultAvatarForegroundColor;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}
