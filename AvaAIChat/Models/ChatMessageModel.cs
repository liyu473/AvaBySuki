using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AvaAIChat.Models;

/// <summary>
/// 聊天消息模型，支持推理过程显示
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
    private ObservableCollection<string> _reasoningSteps = new();

    [ObservableProperty]
    private bool _hasReasoning;

    [ObservableProperty]
    private bool _isReasoningExpanded;

    [ObservableProperty]
    private DateTime _timestamp = DateTime.Now;

    /// <summary>
    /// 添加推理步骤
    /// </summary>
    public void AddReasoningStep(string step)
    {
        ReasoningSteps.Add(step);
        HasReasoning = true;
    }
}
