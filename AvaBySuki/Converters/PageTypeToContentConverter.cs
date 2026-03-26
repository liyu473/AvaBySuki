using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AvaBySuki.Converters;

/// <summary>
/// 将页面类型转换为页面实例的转换器
/// 每次转换都从 DI 容器获取新实例
/// </summary>
public class PageTypeToContentConverter : IValueConverter
{
    public static readonly PageTypeToContentConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Type pageType)
        {
            return App.Services?.GetService(pageType) as UserControl;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
