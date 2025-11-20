using Avalonia.Controls;

namespace AvaBySuki.Models;

/// <summary>
/// 页面信息模型
/// </summary>
public class PageInfo
{
    /// <summary>
    /// 页面显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 图标名称（Material Design Icons）
    /// </summary>
    public string Icon { get; set; } = "mdi-home";

    /// <summary>
    /// 页面内容
    /// </summary>
    public UserControl? PageContent { get; set; }
}