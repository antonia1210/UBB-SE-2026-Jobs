using System;
using Microsoft.UI.Xaml.Data;

namespace UBB_SE_2026_Jobs.App.Converters;

public class BoolNegationConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is not true;

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => value is not true;
}
