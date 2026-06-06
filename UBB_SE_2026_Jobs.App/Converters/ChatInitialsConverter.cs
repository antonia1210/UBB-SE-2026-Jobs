using Microsoft.UI.Xaml.Data;
using UBB_SE_2026_Jobs.App.ViewModels;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.App.Converters;

public class ChatInitialsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        string? name = value switch
        {
            ContactSearchResultViewModel viewModel => viewModel.DisplayName,
            Chat chat => ChatDisplayResolver.ResolveChatName(chat),
            User user => user.Name,
            Company company => company.Name,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(name)) return "?";

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 1
            ? parts[0][0].ToString().ToUpperInvariant()
            : string.Concat(parts[0][0], parts[1][0]).ToUpperInvariant();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotImplementedException();
}
