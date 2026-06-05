using System;
using Microsoft.UI.Xaml.Data;

namespace UBB_SE_2026_Jobs.App.Converters;

public class StringFormatConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        // Check if the value is a date, and a format string was provided in XAML
        if (value is DateTime dateTime && parameter is string format)
        {
            return dateTime.ToString(format);
        }

        // Fallback if something else is passed
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}