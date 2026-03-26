using AvaBySuki.ViewModels;
using LyuExtensions.Aspects;
using Metalama.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ZLogger;

namespace AvaBySuki.Services;

/// <summary>
/// 初始化服务
/// 应用启动时执行相关初始化工作
/// </summary>
[HostedService]
public class InitializationService : IHostedService
{
    [Inject]
    private readonly ILogger<InitializationService> _logger;

    [Inject]
    private readonly IServiceProvider serviceProvider;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.ZLogInformation($"初始化服务启动");
        

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.ZLogInformation($"初始化服务停止");
        return Task.CompletedTask;
    }
}
