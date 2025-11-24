using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.IO;
using AvaBySuki.Services;
using AvaBySuki.ViewModels;
using AvaBySuki.Views;
using LogExtension;

namespace AvaBySuki;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized yet
    // and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            var configuration = BuildConfiguration();
            var host = BuildHost(args, configuration);

            App.Services = host.Services;

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"应用程序启动失败: {ex.Message}");
            Console.WriteLine($"异常类型: {ex.GetType().Name}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"内部异常: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    /// <summary>
    /// 构建配置
    /// </summary>
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    /// <summary>
    /// 构建 Host（依赖注入容器）
    /// </summary>
    private static IHost BuildHost(string[] args, IConfiguration configuration)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddZLogger();
                
                // 注册配置
                services.AddSingleton(configuration);

                // 注册 SukiUI 服务
                services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
                services.AddSingleton<ISukiToastManager, SukiToastManager>();

                // 主题服务
                services.AddSingleton<IThemeConfigService, ThemeConfigService>();

                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<MainWindow>();

                services.AddSingleton<HomePageViewModel>();
                services.AddSingleton<HomePage>();
                
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<SettingsPage>();
                
                services.AddSingleton<WebPageViewModel>();
                services.AddSingleton<WebViewPage>();
                
                services.AddSingleton<ImagePaletteViewModel>();
                services.AddSingleton<ImagePalettePage>();
                
                services.AddTransient<AboutPage>();
            })
            .Build();
    }

    /// <summary>
    /// Avalonia 配置
    /// </summary>
    private static AppBuilder BuildAvaloniaApp()
    {
        // 注册图标提供程序
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}