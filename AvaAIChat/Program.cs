using Avalonia;
using AvaAIChat.Extensions;
using AvaAIChat.Models;
using AvaAIChat.Services;
using AvaAIChat.ViewModels;
using AvaAIChat.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Serilog;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.IO;

namespace AvaAIChat;

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
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            var logger = Log.ForContext<Program>();
            logger.Info($"(环境{environment})应用程序启动...");

            var host = BuildHost(args, configuration);

            App.Services = host.Services;

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);

            logger.Info("应用程序正常退出");
        }
        catch (Exception ex)
        {
            Log.ForContext<Program>().Fatal(ex, "应用程序启动失败");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
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
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // 注册配置
                services.AddSingleton(configuration);

                // 注册 OpenRouter 配置
                services.Configure<OpenRouterConfig>(configuration.GetSection("OpenRouter"));

                // 注册 OpenRouter 服务（使用 OpenAI SDK）
                services.AddSingleton<IOpenRouterService, OpenRouterService>();

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
                services.AddTransient<AboutPage>();
                
                services.AddSingleton<ChatPage>();
                services.AddSingleton<ChatViewModel>();
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