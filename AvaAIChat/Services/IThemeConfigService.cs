using AvaAIChat.Models;
using System.Threading.Tasks;

namespace AvaAIChat.Services;

/// <summary>
/// 主题配置服务接口
/// </summary>
public interface IThemeConfigService
{
    /// <summary>
    /// 加载主题设置
    /// </summary>
    Task<ThemeSettings?> LoadSettingsAsync();

    /// <summary>
    /// 保存主题设置
    /// </summary>
    Task SaveSettingsAsync(ThemeSettings settings);
}