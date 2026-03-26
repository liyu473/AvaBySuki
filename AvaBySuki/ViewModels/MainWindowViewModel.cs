using AvaBySuki.Models;
using AvaBySuki.Views;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using LyuExtensions.Aspects;

namespace AvaBySuki.ViewModels;

[Singleton]
public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// 页面集合
    /// </summary>
    [ObservableProperty]
    public partial AvaloniaList<PageInfo> Pages { get; set; } = [];

    /// <summary>
    /// 当前选中的页面
    /// </summary>
    [ObservableProperty]
    public partial PageInfo? ActivePage { get; set; }

    public MainWindowViewModel()
    {
        InitializePages();
    }

    /// <summary>
    /// 初始化页面数据
    /// </summary>
    private void InitializePages()
    {
        Pages.Add(new PageInfo
        {
            DisplayName = "首页",
            Icon = "mdi-home",
            PageType = typeof(HomePage)
        });

        Pages.Add(new PageInfo
        {
            DisplayName = "设置",
            Icon = "mdi-cog",
            PageType = typeof(SettingsPage)
        });

        Pages.Add(new PageInfo
        {
            DisplayName = "关于",
            Icon = "mdi-information",
            PageType = typeof(AboutPage)
        });

        Pages.Add(new PageInfo
        {
            DisplayName = "网页",
            Icon = "mdi-web",
            PageType = typeof(WebViewPage)
        });

        Pages.Add(new PageInfo
        {
            DisplayName = "图片颜色",
            Icon = "mdi-palette",
            PageType = typeof(ImagePalettePage)
        });

        ActivePage = Pages[0];
    }
}