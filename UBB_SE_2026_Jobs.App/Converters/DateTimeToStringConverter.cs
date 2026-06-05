using System;
using Microsoft.UI.Xaml.Data;

namespace UBB_SE_2026_Jobs.App.Converters;

public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is DateTime dt ? dt.ToLocalTime().ToString("dd MMM yyyy, HH:mm") : string.Empty;

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotImplementedException();
}