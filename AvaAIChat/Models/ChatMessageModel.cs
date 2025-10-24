using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaAIChat.Models;

/// <summary>
/// 聊天消息模型
/// </summary>
public partial class ChatMessageModel : ObservableObject
{
    [ObservableProperty]
    private string _content = string.Empty;

    [ObservableProperty]
    private bool _isUser;

    [ObservableProperty]
    private bool _isThinking;

    [ObservableProperty]
    private string? _avatarPath;

    [ObservableProperty]
    private DateTime _timestamp = DateTime.Now;
}
