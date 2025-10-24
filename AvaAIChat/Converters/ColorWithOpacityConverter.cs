using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AvaAIChat.Converters;

/// <summary>
/// 颜色透明度转换器 - 将颜色转换为指定透明度的颜色
/// </summary>
public class ColorWithOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            // 默认透明度为 0.25 (25%)
            double opacity = 0.25;
            
            if (parameter is string opacityStr && double.TryParse(opacityStr, out double parsedOpacity))
            {
                opacity = parsedOpacity;
            }
            
            // 创建新的颜色，保持 RGB 值，修改 Alpha 通道
            byte alpha = (byte)(255 * opacity);
            return new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
        }
        
        if (value is SolidColorBrush brush)
        {
            var bc = brush.Color;
            double opacity = 0.25;
            
            if (parameter is string opacityStr && double.TryParse(opacityStr, out double parsedOpacity))
            {
                opacity = parsedOpacity;
            }
            
            byte alpha = (byte)(255 * opacity);
            return new SolidColorBrush(Color.FromArgb(alpha, bc.R, bc.G, bc.B));
        }
        
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
