using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using AvaBySuki.Models;
using AvaBySuki.Views;

namespace AvaBySuki.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// 页面集合
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<PageInfo> _pages = [];

    /// <summary>
    /// 当前选中的页面
    /// </summary>
    [ObservableProperty]
    private PageInfo? _activePage;

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
            PageContent = App.GetRequiredService<HomePage>()
        });

        Pages.Add(new PageInfo
        {
            DisplayName = "设置",
            Icon = "mdi-cog",
            PageContent = App.GetRequiredService<SettingsPage>()
        });

        Pages.Add(new PageInfo
        {
            DisplayName = "关于",
            Icon = "mdi-information",
            PageContent = App.GetRequiredService<AboutPage>()
        });
        
        Pages.Add(new PageInfo
        {
            DisplayName = "网页",
            Icon = "mdi-web",
            PageContent = App.GetRequiredService<WebViewPage>()
        });
        
        Pages.Add(new PageInfo
        {
            DisplayName = "图片颜色",
            Icon = "mdi-palette",
            PageContent = App.GetRequiredService<ImagePalettePage>()
        });

        ActivePage = Pages[0];
    }
}