using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ZLogger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using AvaBySuki.Views;

namespace AvaBySuki;

public partial class App : Application
{
    /// <summary>
    /// 依赖注入服务提供者
    /// </summary>
    public static IServiceProvider? Services { get; set; }

    /// <summary>
    /// 获取服务（泛型方法）
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    /// <exception cref="InvalidOperationException">服务未注册或服务提供者未初始化</exception>
    public static T GetService<T>() where T : class
    {
        if (Services == null)
        {
            throw new InvalidOperationException("服务提供者尚未初始化。");
        }

        var service = Services.GetService<T>();
        if (service == null)
        {
            throw new InvalidOperationException($"服务 {typeof(T).Name} 未注册。");
        }

        return service;
    }

    /// <summary>
    /// 尝试获取服务（泛型方法，不会抛出异常）
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例，如果未找到则返回 null</returns>
    public static T? TryGetService<T>() where T : class
    {
        return Services?.GetService<T>();
    }

    /// <summary>
    /// 获取必需服务（泛型方法）
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    /// <exception cref="InvalidOperationException">服务未注册或服务提供者未初始化</exception>
    public static T GetRequiredService<T>() where T : class
    {
        if (Services == null)
        {
            throw new InvalidOperationException("服务提供者尚未初始化。");
        }

        return Services.GetRequiredService<T>();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            // 设置全局异常处理
            SetupExceptionHandlers();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
                DisableAvaloniaDataAnnotationValidation();

                desktop.MainWindow = GetRequiredService<MainWindow>();

                desktop.MainWindow.Closed += OnMainWindowClosed;
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            Log.ForContext<App>().Fatal(ex, "应用程序初始化失败");
            throw;
        }
    }

    /// <summary>
    /// 设置全局异常处理
    /// </summary>
    private void SetupExceptionHandlers()
    {
        var logger = Log.ForContext<App>();

        //处理未捕获的异常（非 UI 线程）
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var exception = e.ExceptionObject as Exception;
            logger.Fatal(exception, "发生未处理的异常（AppDomain）");

            if (e.IsTerminating)
            {
                logger.Fatal("应用程序即将终止");
            }
        };

        //处理未观察到的 Task 异常
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            logger.Error(e.Exception, "发生未观察到的 Task 异常");
            e.SetObserved(); // 防止程序崩溃
        };

        //处理 Avalonia 绑定错误
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
        {
            
        }

        logger.ZLogInformation("全局异常处理已配置");
    }

    /// <summary>
    /// 禁用 Avalonia 的数据注解验证（避免与 CommunityToolkit 冲突）
    /// </summary>
    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove = BindingPlugins
            .DataValidators.OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    /// <summary>
    /// 主窗口关闭事件处理
    /// </summary>
    private void OnMainWindowClosed(object? sender, EventArgs e)
    {
        Log.ForContext<App>().Info("主窗口已关闭");
    }
}