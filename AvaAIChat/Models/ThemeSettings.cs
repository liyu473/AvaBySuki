using System.Collections.Generic;

namespace AvaAIChat.Models;

/// <summary>
/// 主题设置模型
/// </summary>
public class ThemeSettings
{
    /// <summary>
    /// 主题类型（Light/Dark）
    /// </summary>
    public string ThemeVariant { get; set; } = "Dark";

    /// <summary>
    /// 选中的颜色主题名称
    /// </summary>
    public string? SelectedColorThemeName { get; set; }

    /// <summary>
    /// 自定义颜色列表
    /// </summary>
    public List<CustomColorTheme> CustomColorThemes { get; set; } = [];
}

/// <summary>
/// 自定义颜色主题数据
/// </summary>
public class CustomColorTheme
{
    /// <summary>
    /// 主题名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 主色调（ARGB格式）
    /// </summary>
    public uint PrimaryColor { get; set; }

    /// <summary>
    /// 强调色（ARGB格式）
    /// </summary>
    public uint AccentColor { get; set; }
}