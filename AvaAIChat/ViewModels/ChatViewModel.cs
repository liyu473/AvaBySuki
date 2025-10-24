using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaAIChat.Models;
using AvaAIChat.Services;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaAIChat.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    private readonly IOpenRouterService _openRouterService;
    
    [ObservableProperty]
    private ObservableCollection<ChatMessageModel> _messages = new();

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isSending = false;

    private CancellationTokenSource? _cancellationTokenSource;

    public ChatViewModel(IOpenRouterService openRouterService)
    {
        _openRouterService = openRouterService;
        
        // 添加欢迎消息
        Messages.Add(new ChatMessageModel
        {
            Content = "你好！我是AI助手，有什么可以帮助你的吗？",
            IsUser = false,
            Timestamp = DateTime.Now
        });
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText) || IsSending)
            return;

        var userMessage = InputText;
        InputText = string.Empty;
        
        var userMsg = new ChatMessageModel
        {
            Content = userMessage,
            IsUser = true,
            Timestamp = DateTime.Now
        };
        Messages.Add(userMsg);
        
        await GenerateAIResponseAsync(userMessage);
    }

    [RelayCommand]
    private void StopGeneration()
    {
        _cancellationTokenSource?.Cancel();
        IsSending = false;
    }

    private async Task GenerateAIResponseAsync(string userInput)
    {
        IsSending = true;
        _cancellationTokenSource = new CancellationTokenSource();

        // 创建AI消息（思考中状态）
        var aiMessage = new ChatMessageModel
        {
            Content = string.Empty,
            IsUser = false,
            IsThinking = true,
            Timestamp = DateTime.Now
        };

        await Dispatcher.UIThread.InvokeAsync(() => Messages.Add(aiMessage));

        try
        {
            await SimulateAIResponseAsync(aiMessage, userInput, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            // 用户取消了生成
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                aiMessage.IsThinking = false;
                aiMessage.Content = "[已停止生成]";
            });
        }
        catch (Exception ex)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                aiMessage.IsThinking = false;
                aiMessage.Content = $"错误: {ex.Message}";
            });
        }
        finally
        {
            IsSending = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// 调用 OpenRouter API 获取响应（流式）
    /// </summary>
    private async Task SimulateAIResponseAsync(ChatMessageModel aiMessage, string userInput, CancellationToken cancellationToken)
    {
        try
        {
            // 构建对话历史
            var messages = new List<OpenRouterMessage>();
            
            // 添加历史消息（最多取最近10条）
            var recentMessages = Messages
                .Where(m => !m.IsThinking && !string.IsNullOrEmpty(m.Content))
                .TakeLast(10)
                .ToList();
            
            foreach (var msg in recentMessages)
            {
                messages.Add(new OpenRouterMessage
                {
                    Role = msg.IsUser ? "user" : "assistant",
                    Content = msg.Content
                });
            }
            
            // 如果历史中没有当前用户消息，添加它
            if (!messages.Any() || messages.Last().Content != userInput)
            {
                messages.Add(new OpenRouterMessage
                {
                    Role = "user",
                    Content = userInput
                });
            }

            // 调用流式 API
            bool isFirstChunk = true;
            
            await foreach (var chunk in _openRouterService.SendChatStreamAsync(messages, cancellationToken))
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // 收到第一个响应块时，关闭思考状态
                    if (isFirstChunk)
                    {
                        aiMessage.IsThinking = false;
                        aiMessage.Content = string.Empty;
                        isFirstChunk = false;
                    }
                    
                    aiMessage.Content += chunk;
                });
            }
        }
        catch (OperationCanceledException)
        {
            throw; // 让外层处理
        }
        catch (Exception ex)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                aiMessage.IsThinking = false;
                aiMessage.Content = $"API 调用失败: {ex.Message}\n\n请检查:\n1. API Key 是否正确\n2. 网络连接是否正常\n3. OpenRouter 配额是否充足";
            });
        }
    }
}