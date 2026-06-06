using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace UBB_SE_2026_Jobs.App.Converters;

public class StatusToColorConverter : IValueConverter
{
    private static readonly Color ScheduledColor = Color.FromArgb(255, 59, 130, 246);
    private static readonly Color CompletedColor = Color.FromArgb(255, 34, 197, 94);
    private static readonly Color CancelledColor = Color.FromArgb(255, 239, 68, 68);
    private static readonly Color DefaultColor = Color.FromArgb(255, 107, 114, 128);

    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        var color = (value as string) switch
        {
            "Scheduled" => ScheduledColor,
            "Completed" => CompletedColor,
            "Cancelled" => CancelledColor,
            _ => DefaultColor
        };
        return new SolidColorBrush(color);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotImplementedException();
}