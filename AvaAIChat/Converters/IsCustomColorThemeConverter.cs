using Avalonia.Data.Converters;
using AvaAIChat.ViewModels;
using SukiUI.Models;
using System;
using System.Globalization;

namespace AvaAIChat.Converters;

/// <summary>
/// 检查颜色主题是否为自定义主题的转换器
/// </summary>
public class IsCustomColorThemeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SukiColorTheme colorTheme && parameter is SettingsViewModel viewModel)
        {
            return viewModel.CustomColorThemes.Contains(colorTheme);
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}