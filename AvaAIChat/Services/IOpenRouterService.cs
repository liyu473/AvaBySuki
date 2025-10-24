using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AvaAIChat.Models;

namespace AvaAIChat.Services;

/// <summary>
/// OpenRouter API 服务接口
/// </summary>
public interface IOpenRouterService
{
    /// <summary>
    /// 发送聊天请求（非流式）
    /// </summary>
    Task<string> SendChatAsync(List<OpenRouterMessage> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送聊天请求（流式）
    /// </summary>
    IAsyncEnumerable<string> SendChatStreamAsync(List<OpenRouterMessage> messages, CancellationToken cancellationToken = default);
}
