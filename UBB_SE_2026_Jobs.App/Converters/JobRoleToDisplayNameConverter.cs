using System;
using Microsoft.UI.Xaml.Data;
using UBB_SE_2026_Jobs.App.ViewModels;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.App.Converters;

public class JobRoleToDisplayNameConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is JobRole role ? ViewModelSupport.FormatJobRole(role) : value?.ToString() ?? string.Empty;

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotImplementedException();
}
