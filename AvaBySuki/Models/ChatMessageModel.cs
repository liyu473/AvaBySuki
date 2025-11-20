using System;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaBySuki.Models;

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
    
    /// <summary>
    /// 流式内容是否完成
    /// </summary>
    [ObservableProperty]
    private bool _isStreaming = false;
    
    // 流式内容缓冲区
    private StringBuilder? _streamBuffer;
    
    /// <summary>
    /// 开始流式更新
    /// </summary>
    public void StartStreaming()
    {
        IsStreaming = true;
        _streamBuffer = new StringBuilder();
    }
    
    /// <summary>
    /// 追加流式内容（不立即触发 UI 更新）
    /// </summary>
    public void AppendStreamContent(string chunk)
    {
        _streamBuffer?.Append(chunk);
    }
    
    /// <summary>
    /// 刷新缓冲区到 Content（触发 UI 更新）
    /// </summary>
    public void FlushStreamBuffer()
    {
        if (_streamBuffer != null)
        {
            Content = _streamBuffer.ToString();
        }
    }
    
    /// <summary>
    /// 完成流式更新
    /// </summary>
    public void FinishStreaming()
    {
        FlushStreamBuffer();
        IsStreaming = false;
        _streamBuffer = null;
    }
}
