using System;
using System.Collections.ObjectModel;
using ZLogger;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using AvaBySuki.Extensions;
using AvaBySuki.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PaletteNet;
using SukiUI.Toasts;

namespace AvaBySuki.ViewModels;

public partial class ImagePaletteViewModel : ViewModelBase
{
    private readonly ILogger<ImagePaletteViewModel> _logger;
    private readonly ISukiToastManager _toastManager;

    [ObservableProperty]
    private Bitmap? _selectedImage;

    [ObservableProperty]
    private string? _imagePath;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private ObservableCollection<ColorInfo> _extractedColors = new();

    [ObservableProperty]
    private ColorInfo? _dominantColor;

    [ObservableProperty]
    private ColorInfo? _vibrantColor;

    [ObservableProperty]
    private ColorInfo? _mutedColor;

    [ObservableProperty]
    private ColorInfo? _lightVibrantColor;

    [ObservableProperty]
    private ColorInfo? _darkVibrantColor;

    [ObservableProperty]
    private ColorInfo? _lightMutedColor;

    [ObservableProperty]
    private ColorInfo? _darkMutedColor;

    public ImagePaletteViewModel(
        ILogger<ImagePaletteViewModel> logger,
        ISukiToastManager toastManager)
    {
        _logger = logger;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private async Task SelectImage()
    {
        try
        {
            var topLevel = Avalonia.Application.Current?.ApplicationLifetime
                is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

            if (topLevel == null)
            {
                _toastManager.ShowErrorShort("无法获取主窗口");
                return;
            }

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择图片",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("图片文件")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.webp" }
                    }
                }
            });

            if (files.Count > 0)
            {
                var file = files[0];
                ImagePath = file.Path.LocalPath;

                // 加载图片
                SelectedImage = new Bitmap(ImagePath);

                // 提取颜色
                await ExtractColors();

                _toastManager.ShowSuccessShort("图片加载成功");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "选择图片时出错");
            _toastManager.ShowError("错误", $"加载图片失败: {ex.Message}");
        }
    }

    private async Task ExtractColors()
    {
        if (SelectedImage == null)
            return;

        IsProcessing = true;

        try
        {
            await Task.Run(() =>
            {
                // 使用 Avalonia Bitmap Helper 适配器
                var bitmapHelper = new AvaloniaBitmapHelper(SelectedImage);

                // 创建 Palette Builder
                var paletteBuilder = new PaletteBuilder();
                var palette = paletteBuilder.Generate(bitmapHelper);

                // 提取各种颜色
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    ExtractedColors.Clear();

                    // 主色调
                    if (palette.DominantSwatch != null)
                    {
                        DominantColor = CreateColorInfo("主色调", palette.DominantSwatch);
                        ExtractedColors.Add(DominantColor);
                    }

                    // 鲜艳色
                    if (palette.VibrantSwatch != null)
                    {
                        VibrantColor = CreateColorInfo("鲜艳色", palette.VibrantSwatch);
                        ExtractedColors.Add(VibrantColor);
                    }

                    // 柔和色
                    if (palette.MutedSwatch != null)
                    {
                        MutedColor = CreateColorInfo("柔和色", palette.MutedSwatch);
                        ExtractedColors.Add(MutedColor);
                    }

                    // 浅鲜艳色
                    if (palette.LightVibrantSwatch != null)
                    {
                        LightVibrantColor = CreateColorInfo("浅鲜艳色", palette.LightVibrantSwatch);
                        ExtractedColors.Add(LightVibrantColor);
                    }

                    // 深鲜艳色
                    if (palette.DarkVibrantSwatch != null)
                    {
                        DarkVibrantColor = CreateColorInfo("深鲜艳色", palette.DarkVibrantSwatch);
                        ExtractedColors.Add(DarkVibrantColor);
                    }

                    // 浅柔和色
                    if (palette.LightMutedSwatch != null)
                    {
                        LightMutedColor = CreateColorInfo("浅柔和色", palette.LightMutedSwatch);
                        ExtractedColors.Add(LightMutedColor);
                    }

                    // 深柔和色
                    if (palette.DarkMutedSwatch != null)
                    {
                        DarkMutedColor = CreateColorInfo("深柔和色", palette.DarkMutedSwatch);
                        ExtractedColors.Add(DarkMutedColor);
                    }
                });
            });

            _toastManager.ShowSuccessShort($"成功提取 {ExtractedColors.Count} 种颜色");
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"提取颜色时出错: {ex.Message}");
            _toastManager.ShowError("错误", $"提取颜色失败: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private ColorInfo CreateColorInfo(string name, Swatch swatch)
    {
        var rgb = swatch.Rgb;
        var r = (byte)((rgb >> 16) & 0xFF);
        var g = (byte)((rgb >> 8) & 0xFF);
        var b = (byte)(rgb & 0xFF);

        var color = Color.FromRgb(r, g, b);
        var titleColor = ConvertToAvaloniaColor(swatch.TitleTextColor);
        var bodyColor = ConvertToAvaloniaColor(swatch.BodyTextColor);

        return new ColorInfo
        {
            Name = name,
            HexColor = $"#{r:X2}{g:X2}{b:X2}",
            Color = color,
            ColorBrush = new SolidColorBrush(color),
            Population = swatch.Population,
            TitleTextColor = titleColor,
            TitleTextBrush = new SolidColorBrush(titleColor),
            BodyTextColor = bodyColor,
            BodyTextBrush = new SolidColorBrush(bodyColor)
        };
    }

    private Color ConvertToAvaloniaColor(int argb)
    {
        var a = (byte)((argb >> 24) & 0xFF);
        var r = (byte)((argb >> 16) & 0xFF);
        var g = (byte)((argb >> 8) & 0xFF);
        var b = (byte)(argb & 0xFF);
        return Color.FromArgb(a, r, g, b);
    }

    [RelayCommand]
    private async Task CopyColorCode(ColorInfo colorInfo)
    {
        if (colorInfo?.HexColor != null)
        {
            try
            {
                var topLevel = Avalonia.Application.Current?.ApplicationLifetime
                    is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

                if (topLevel?.Clipboard != null)
                {
                    await topLevel.Clipboard.SetTextAsync(colorInfo.HexColor);
                    _toastManager.ShowSuccessShort($"已复制: {colorInfo.HexColor}");
                }
            }
            catch (Exception ex)
            {
                _logger.ZLogError(ex, $"复制颜色代码时出错");
                _toastManager.ShowErrorShort("复制失败");
            }
        }
    }
}

public class ColorInfo
{
    public string Name { get; set; } = string.Empty;
    public string HexColor { get; set; } = string.Empty;
    public Color Color { get; set; }
    public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush();
    public int Population { get; set; }
    public Color TitleTextColor { get; set; }
    public SolidColorBrush TitleTextBrush { get; set; } = new SolidColorBrush();
    public Color BodyTextColor { get; set; }
    public SolidColorBrush BodyTextBrush { get; set; } = new SolidColorBrush();
}
