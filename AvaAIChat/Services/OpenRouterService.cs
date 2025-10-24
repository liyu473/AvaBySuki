using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AvaAIChat.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace AvaAIChat.Services;

/// <summary>
/// OpenRouter API 服务实现
/// </summary>
public class OpenRouterService : IOpenRouterService
{
    private readonly OpenAIClient _client;
    private readonly OpenRouterConfig _config;
    private readonly ILogger<OpenRouterService> _logger;

    public OpenRouterService(
        IOptions<OpenRouterConfig> config,
        ILogger<OpenRouterService> _logger)
    {
        _config = config.Value;
        this._logger = _logger;

        // 使用 OpenAI SDK 初始化客户端，指向 OpenRouter
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(_config.BaseUrl)
        };
        
        var credential = new ApiKeyCredential(_config.ApiKey);
        _client = new OpenAIClient(credential, options);
        
        _logger.LogInformation("OpenRouter 服务已初始化 (使用 OpenAI SDK)，BaseUrl: {BaseUrl}, Model: {Model}", _config.BaseUrl, _config.Model);
    }

    /// <summary>
    /// 发送聊天请求（非流式）
    /// </summary>
    public async Task<string> SendChatAsync(List<OpenRouterMessage> messages, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("发送 OpenRouter 请求，模型: {Model}", _config.Model);

            // 转换消息格式
            var chatMessages = new List<ChatMessage>();
            foreach (var m in messages)
            {
                ChatMessage chatMessage = m.Role switch
                {
                    "user" => ChatMessage.CreateUserMessage(m.Content),
                    "assistant" => ChatMessage.CreateAssistantMessage(m.Content),
                    "system" => ChatMessage.CreateSystemMessage(m.Content),
                    _ => ChatMessage.CreateUserMessage(m.Content)
                };
                chatMessages.Add(chatMessage);
            }

            var chatRequest = new ChatCompletionOptions
            {
                MaxOutputTokenCount = _config.MaxTokens,
                Temperature = (float)_config.Temperature
            };

            var chatClient = _client.GetChatClient(_config.Model);
            var response = await chatClient.CompleteChatAsync(chatMessages, chatRequest, cancellationToken);

            var content = response.Value.Content[0].Text;
            _logger.LogInformation("收到响应，长度: {Length}", content?.Length ?? 0);
            return content ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送聊天请求时发生错误");
            
            // 特殊处理 429 错误
            if (ex.Message.Contains("429") || ex.Message.Contains("Too Many Requests"))
            {
                throw new Exception($"速率限制：请求过于频繁\n\n可能原因：\n1. 免费配额已用完（每天有限制）\n2. 短时间内请求过多\n\n建议：\n• 等待几分钟后重试\n• 查看 OpenRouter 控制台的配额使用情况\n• 考虑切换到其他免费模型", ex);
            }
            
            throw new Exception($"API 调用失败: {ex.Message}\n\n请检查:\n1. API Key 是否正确\n2. 网络连接是否正常\n3. OpenRouter 配额是否充足", ex);
        }
    }

    /// <summary>
    /// 发送聊天请求（流式）
    /// </summary>
    public async IAsyncEnumerable<string> SendChatStreamAsync(
        List<OpenRouterMessage> messages,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("发送流式 OpenRouter 请求，模型: {Model}", _config.Model);

        // 转换消息格式
        var chatMessages = new List<ChatMessage>();
        foreach (var m in messages)
        {
            ChatMessage chatMessage = m.Role switch
            {
                "user" => ChatMessage.CreateUserMessage(m.Content),
                "assistant" => ChatMessage.CreateAssistantMessage(m.Content),
                "system" => ChatMessage.CreateSystemMessage(m.Content),
                _ => ChatMessage.CreateUserMessage(m.Content)
            };
            chatMessages.Add(chatMessage);
        }

        var chatRequest = new ChatCompletionOptions
        {
            MaxOutputTokenCount = _config.MaxTokens,
            Temperature = (float)_config.Temperature
        };

        var chatClient = _client.GetChatClient(_config.Model);
        var streamingResponse = chatClient.CompleteChatStreamingAsync(chatMessages, chatRequest, cancellationToken);

        await foreach (var update in streamingResponse.WithCancellation(cancellationToken))
        {
            foreach (var contentPart in update.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(contentPart.Text))
                {
                    yield return contentPart.Text;
                }
            }
        }

        _logger.LogDebug("流式响应完成");
    }
}
