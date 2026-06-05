using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.App.Converters;

public class ChatAvatarCornerRadiusConverter : IValueConverter
{
    private const double CompanyAvatarCornerRadius = 8;
    private const double CircularAvatarCornerRadius = 999;

    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is not Chat chat)
        {
            return new CornerRadius(CircularAvatarCornerRadius);
        }

        return chat.Company!=null
            ? new CornerRadius(CompanyAvatarCornerRadius)
            : new CornerRadius(CircularAvatarCornerRadius);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}
