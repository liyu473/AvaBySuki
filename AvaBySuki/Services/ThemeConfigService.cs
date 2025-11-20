using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AvaBySuki.Models;
using Extensions;
using ZLogger;

namespace AvaBySuki.Services;

/// <summary>
/// 主题配置服务实现
/// </summary>
public class ThemeConfigService : IThemeConfigService
{
    private readonly ILogger<ThemeConfigService> _logger;
    private readonly string _settingsFilePath;

    public ThemeConfigService(ILogger<ThemeConfigService> logger)
    {
        _logger = logger;

        // 获取用户配置目录
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "AvaAIChat");

        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        _settingsFilePath = Path.Combine(appFolder, "theme-settings.json");
    }

    /// <summary>
    /// 加载主题设置
    /// </summary>
    public async Task<ThemeSettings?> LoadSettingsAsync()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                _logger.ZLogWarning($"主题配置文件不存在，返回默认设置");
                return null;
            }

            var json = await File.ReadAllTextAsync(_settingsFilePath);
            var settings = JsonSerializer.Deserialize<ThemeSettings>(json);

            _logger.ZLogInformation($"成功加载主题设置");
            return settings;
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"加载主题设置失败");
            return null;
        }
    }


    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    /// <summary>
    /// 保存主题设置
    /// </summary>
    public async Task SaveSettingsAsync(ThemeSettings settings)
    {
        try
        {
            var json = settings.ToJson(_options);
            await File.WriteAllTextAsync(_settingsFilePath, json);

            _logger.ZLogInformation($"成功保存主题设置到: {_settingsFilePath}");
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"保存主题设置失败");
        }
    }
}