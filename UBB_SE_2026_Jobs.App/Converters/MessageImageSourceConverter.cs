using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using UBB_SE_2026_Jobs.App.Configuration;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.App.Converters;

public class MessageImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is not Message message || message.Type != MessageType.Image || string.IsNullOrWhiteSpace(message.Content))
        {
            return null;
        }

        if (!Uri.TryCreate(message.Content, UriKind.Absolute, out var uri))
        {
            var baseUrl = ApiConfigurationLoader.Load().BaseUrl.TrimEnd('/');
            var fileName = Uri.EscapeDataString(Path.GetFileName(message.Content));
            uri = new Uri($"{baseUrl}/api/files/{fileName}");
        }

        return new BitmapImage(uri);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}
