using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AvaAIChat.Converters;

public class BoolToMessageBgConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isUser)
        {
            // 用户消息使用蓝色背景，AI消息使用灰色背景
            return isUser 
                ? new SolidColorBrush(Color.Parse("#3B82F6")) 
                : new SolidColorBrush(Color.Parse("#374151"));
        }
        return new SolidColorBrush(Color.Parse("#374151"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
