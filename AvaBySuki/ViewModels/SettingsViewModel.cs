using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using AvaBySuki.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SukiUI;
using SukiUI.Dialogs;
using SukiUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaBySuki.Models;
using AvaBySuki.Services;
using ZLogger;

namespace AvaBySuki.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly SukiTheme _sukiTheme;
    private readonly ILogger<SettingsViewModel> _logger;
    private readonly ISukiDialogManager _dialogManager;
    private readonly IThemeConfigService _themeConfigService;

    /// <summary>
    /// 标志是否正在初始化（防止在加载设置时自动保存）
    /// </summary>
    private bool _isInitializing = true;

    [ObservableProperty]
    private ThemeVariant _currentTheme;

    [ObservableProperty]
    private SukiColorTheme? _selectedColorTheme;

    [ObservableProperty]
    private Color _customColor = Colors.DeepSkyBlue;

    [ObservableProperty]
    private Color _dialogCustomColor = Colors.DeepSkyBlue;

    /// <summary>
    /// 可用的主题颜色列表（包含系统自带和自定义）
    /// </summary>
    public ObservableCollection<SukiColorTheme> AvailableColors { get; }

    /// <summary>
    /// 自定义颜色主题列表（用于持久化）
    /// </summary>
    public ObservableCollection<SukiColorTheme> CustomColorThemes { get; } = new();

    /// <summary>
    /// 当前主题显示名称
    /// </summary>
    public string CurrentThemeName => CurrentTheme == ThemeVariant.Dark ? "深色" : "浅色";

    /// <summary>
    /// 当前是否为深色主题（用于 ToggleSwitch）
    /// </summary>
    public bool IsDarkTheme
    {
        get => CurrentTheme == ThemeVariant.Dark;
        set
        {
            if (value != IsDarkTheme)
            {
                ToggleTheme();
            }
        }
    }

    public SettingsViewModel(
        ILogger<SettingsViewModel> logger,
        ISukiDialogManager dialogManager,
        IThemeConfigService themeConfigService)
    {
        _logger = logger;
        _dialogManager = dialogManager;
        _themeConfigService = themeConfigService;
        _sukiTheme = SukiTheme.GetInstance();

        // 初始化可用颜色列表（系统自带）
        AvailableColors = new ObservableCollection<SukiColorTheme>(_sukiTheme.ColorThemes);

        // 设置主题
        CurrentTheme = _sukiTheme.ActiveBaseTheme;
        SelectedColorTheme = _sukiTheme.ActiveColorTheme;

        // 加载保存的设置
        _ = LoadSettingsAsync();
    }

    /// <summary>
    /// 切换主题（亮色/暗色）
    /// </summary>
    [RelayCommand]
    private void ToggleTheme()
    {
        var currentColorTheme = SelectedColorTheme;

        var newTheme = CurrentTheme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
        _sukiTheme.SwitchBaseTheme();
        CurrentTheme = newTheme;

        if (currentColorTheme != null)
        {
            _sukiTheme.ChangeColorTheme(currentColorTheme);
        }

        _logger.ZLogInformation($"主题已切换到: {CurrentThemeName}");
        OnPropertyChanged(nameof(CurrentThemeName));
        OnPropertyChanged(nameof(IsDarkTheme));
    }

    /// <summary>
    /// 切换到浅色主题
    /// </summary>
    [RelayCommand]
    private void SwitchToLight()
    {
        if (CurrentTheme != ThemeVariant.Light)
        {
            var currentColorTheme = SelectedColorTheme;

            _sukiTheme.ChangeBaseTheme(ThemeVariant.Light);
            CurrentTheme = ThemeVariant.Light;

            if (currentColorTheme != null)
            {
                _sukiTheme.ChangeColorTheme(currentColorTheme);
            }

            _logger.ZLogInformation($"切换到浅色主题");
            OnPropertyChanged(nameof(CurrentThemeName));
            OnPropertyChanged(nameof(IsDarkTheme));
        }
    }

    /// <summary>
    /// 切换到深色主题
    /// </summary>
    [RelayCommand]
    private void SwitchToDark()
    {
        if (CurrentTheme != ThemeVariant.Dark)
        {
            var currentColorTheme = SelectedColorTheme;

            _sukiTheme.ChangeBaseTheme(ThemeVariant.Dark);
            CurrentTheme = ThemeVariant.Dark;
            if (currentColorTheme != null)
            {
                _sukiTheme.ChangeColorTheme(currentColorTheme);
            }

            _logger.ZLogInformation($"切换到深色主题");
            OnPropertyChanged(nameof(CurrentThemeName));
            OnPropertyChanged(nameof(IsDarkTheme));
        }
    }

    /// <summary>
    /// 切换颜色主题
    /// </summary>
    [RelayCommand]
    private void ChangeColorTheme(SukiColorTheme colorTheme)
    {
        SelectedColorTheme = colorTheme;
    }

    /// <summary>
    /// 应用自定义颜色
    /// </summary>
    [RelayCommand]
    private void ApplyCustomColor()
    {
        // 创建自定义颜色主题（主色和强调色使用相同颜色）
        var customTheme = new SukiColorTheme("Custom", CustomColor, CustomColor);

        // 检查是否已有自定义主题
        var existingCustom = AvailableColors.FirstOrDefault(c => c.DisplayName == "Custom");
        if (existingCustom == null)
        {
            _sukiTheme.AddColorTheme(customTheme);
        }

        _sukiTheme.ChangeColorTheme(customTheme);
        SelectedColorTheme = customTheme;

        _logger.ZLogInformation($"应用自定义颜色: {CustomColor}");
    }

    /// <summary>
    /// 显示添加自定义颜色对话框
    /// </summary>
    [RelayCommand]
    private void ShowAddCustomColorDialog()
    {
        var colorPicker = new ColorPicker.StandardColorPicker
        {
            Width = 300,
            Height = 400,
            ShowAlpha = false,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        try
        {
            colorPicker.Color = new ColorPicker.Models.NotifyableColor(colorPicker)
            {
                RGB_R = DialogCustomColor.R,
                RGB_G = DialogCustomColor.G,
                RGB_B = DialogCustomColor.B
            };
            _logger.ZLogInformation($"初始化颜色选择器，颜色: {DialogCustomColor}");
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"初始化颜色选择器失败");
        }

        // 监听颜色变化
        colorPicker.PropertyChanged += (sender, e) =>
        {
            if (e.Property == ColorPicker.PickerControlBase.ColorProperty &&
                e.NewValue is ColorPicker.Models.NotifyableColor notifyableColor)
            {
                try
                {
                    var newColor = Color.FromArgb(255,
                        (byte)notifyableColor.RGB_R,
                        (byte)notifyableColor.RGB_G,
                        (byte)notifyableColor.RGB_B);
                    DialogCustomColor = newColor;
                    _logger.ZLogInformation($"颜色已更新: {newColor}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "更新颜色失败");
                }
            }
        };

        var content = new StackPanel
        {
            Spacing = 20,
            Children =
            {
                new TextBlock
                {
                    Text = "选择您喜欢的颜色作为主题色",
                    FontSize = 15,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeight.Medium
                },
                colorPicker
            }
        };

        _dialogManager.CreateDialog()
            .WithTitle("添加自定义颜色")
            .WithContent(content)
            .WithActionButton("应用", _ =>
            {
                try
                {
                    if (colorPicker.Color is { } finalColor)
                    {
                        DialogCustomColor = Color.FromArgb(255,
                            (byte)finalColor.RGB_R,
                            (byte)finalColor.RGB_G,
                            (byte)finalColor.RGB_B);
                        _logger.ZLogInformation($"从颜色选择器读取颜色: {DialogCustomColor}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.ZLogError(ex, $"读取颜色选择器颜色失败");
                }

                _logger.ZLogInformation($"点击应用按钮，应用颜色: {DialogCustomColor}");
                AddCustomColorToTheme();
            }, true)
            .WithActionButton("取消", _ => { }, true)
            .TryShow();
    }

    /// <summary>
    /// 将对话框中选择的颜色添加到主题列表
    /// </summary>
    private void AddCustomColorToTheme()
    {
        var themeName = $"自定义 {DialogCustomColor}";
        var customTheme = new SukiColorTheme(themeName, DialogCustomColor, DialogCustomColor);

        var existingInTheme = _sukiTheme.ColorThemes.FirstOrDefault(ct => ct.Equals(customTheme) || ct.DisplayName == themeName);
        if (existingInTheme != null)
        {
            if (!AvailableColors.Contains(existingInTheme))
            {
                AvailableColors.Add(existingInTheme);
            }

            // 作为自定义主题纳入持久化
            if (!CustomColorThemes.Contains(existingInTheme) && (existingInTheme.DisplayName?.StartsWith("自定义") == true))
            {
                CustomColorThemes.Add(existingInTheme);
            }

            _sukiTheme.ChangeColorTheme(existingInTheme);
            SelectedColorTheme = existingInTheme;
            _logger.ZLogInformation($"已存在主题，直接切换: {existingInTheme.DisplayName}");
            _ = SaveSettingsAsync();
            return;
        }

        _sukiTheme.AddColorTheme(customTheme);
        AvailableColors.Add(customTheme);
        CustomColorThemes.Add(customTheme);

        _sukiTheme.ChangeColorTheme(customTheme);
        SelectedColorTheme = customTheme;

        _logger.ZLogInformation($"添加并应用自定义颜色: {DialogCustomColor}");
        _ = SaveSettingsAsync();
    }

    /// <summary>
    /// 删除颜色主题
    /// </summary>
    [RelayCommand]
    private void DeleteColorTheme(SukiColorTheme? colorTheme)
    {
        if (colorTheme == null) return;

        // 检查是否是自定义主题
        if (!CustomColorThemes.Contains(colorTheme))
        {
            _logger.ZLogInformation($"无法删除系统自带主题: {colorTheme.DisplayName}");
            return;
        }

        // 如果删除的是当前选中的主题，切换到默认主题
        if (SelectedColorTheme == colorTheme)
        {
            var defaultTheme = AvailableColors.FirstOrDefault();
            if (defaultTheme != null)
            {
                SelectedColorTheme = defaultTheme;
            }
        }

        CustomColorThemes.Remove(colorTheme);
        AvailableColors.Remove(colorTheme);

        _logger.ZLogInformation($"已删除自定义颜色主题: {colorTheme.DisplayName}");

        _ = SaveSettingsAsync();
    }

    /// <summary>
    /// 当选中的颜色改变时
    /// </summary>
    partial void OnSelectedColorThemeChanged(SukiColorTheme? value)
    {
        if (value != null)
        {
            _sukiTheme.ChangeColorTheme(value);
            _logger.ZLogInformation($"主题颜色已切换到: {value.DisplayName}");

            if (!_isInitializing)
            {
                _ = SaveSettingsAsync();
            }
        }
    }

    /// <summary>
    /// 加载保存的主题设置
    /// </summary>
    private async Task LoadSettingsAsync()
    {
        try
        {
            var settings = await _themeConfigService.LoadSettingsAsync();
            if (settings == null)
            {
                _isInitializing = false;
                return;
            }

            // 恢复主题
            var themeVariant = settings.ThemeVariant == "Light" ? ThemeVariant.Light : ThemeVariant.Dark;
            if (themeVariant != CurrentTheme)
            {
                _sukiTheme.ChangeBaseTheme(themeVariant);
                CurrentTheme = themeVariant;
                OnPropertyChanged(nameof(CurrentThemeName));
                OnPropertyChanged(nameof(IsDarkTheme));
            }

            // 恢复自定义颜色主题
            foreach (var customColor in settings.CustomColorThemes)
            {
                var primaryColor = Color.FromUInt32(customColor.PrimaryColor);
                var accentColor = Color.FromUInt32(customColor.AccentColor);
                var theme = new SukiColorTheme(customColor.Name, primaryColor, accentColor);

                _sukiTheme.AddColorTheme(theme);
                AvailableColors.Add(theme);
                CustomColorThemes.Add(theme);
            }

            // 恢复选中的颜色
            if (!string.IsNullOrEmpty(settings.SelectedColorThemeName))
            {
                var selectedTheme = AvailableColors.FirstOrDefault(c =>
                    c.DisplayName == settings.SelectedColorThemeName);
                if (selectedTheme != null)
                {
                    _sukiTheme.ChangeColorTheme(selectedTheme);
                    SelectedColorTheme = selectedTheme;
                }
            }

            _logger.ZLogInformation($"成功恢复主题设置");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载主题设置失败");
        }
        finally
        {
            _isInitializing = false;
        }
    }

    /// <summary>
    /// 保存当前主题设置
    /// </summary>
    private async Task SaveSettingsAsync()
    {
        try
        {
            var settings = new ThemeSettings
            {
                ThemeVariant = CurrentTheme == ThemeVariant.Dark ? "Dark" : "Light",
                SelectedColorThemeName = SelectedColorTheme?.DisplayName,
                CustomColorThemes = CustomColorThemes.Select(theme => new CustomColorTheme
                {
                    Name = theme.DisplayName ?? "Custom",
                    PrimaryColor = theme.Primary.ToUInt32(),
                    AccentColor = theme.Accent.ToUInt32()
                }).ToList()
            };

            await _themeConfigService.SaveSettingsAsync(settings);
            _logger.ZLogInformation($"成功保存主题设置");
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"保存主题设置失败");
        }
    }

    /// <summary>
    /// 当主题类型改变时保存设置
    /// </summary>
    partial void OnCurrentThemeChanged(ThemeVariant value)
    {
        if (!_isInitializing)
        {
            _ = SaveSettingsAsync();
        }
    }
}