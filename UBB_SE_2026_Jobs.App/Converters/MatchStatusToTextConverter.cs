using System;
using Microsoft.UI.Xaml.Data;
using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.App.Converters;

public class MatchStatusToTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
        => value is MatchStatus status
            ? status switch
            {
                MatchStatus.Accepted => "Accepted",
                MatchStatus.Rejected => "Rejected",
                MatchStatus.Advanced => "Advanced",
                _                   => "Applied",
            }
            : "Applied";

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotImplementedException();
}
