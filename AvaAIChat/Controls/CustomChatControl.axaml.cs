using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaAIChat.ViewModels;

namespace AvaAIChat.Controls;

public partial class CustomChatControl : UserControl
{
    private DispatcherTimer? _scrollTimer;
    private bool _needsScroll;
    
    public CustomChatControl()
    {
        InitializeComponent();
        
        // 监听数据上下文变化
        DataContextChanged += OnDataContextChanged;
        
        // 初始化滚动定时器（节流）
        _scrollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _scrollTimer.Tick += (s, e) =>
        {
            if (_needsScroll)
            {
                PerformScroll();
                _needsScroll = false;
            }
        };
        _scrollTimer.Start();
    }
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ChatViewModel viewModel)
        {
            // 监听消息集合变化
            viewModel.Messages.CollectionChanged += Messages_CollectionChanged;
            
            // 为现有消息添加监听
            foreach (var message in viewModel.Messages)
            {
                message.PropertyChanged += Message_PropertyChanged;
            }
        }
    }
    
    private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // 新消息添加时，监听其属性变化
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is AvaAIChat.Models.ChatMessageModel message)
                {
                    message.PropertyChanged += Message_PropertyChanged;
                }
            }
        }
        
        // 滚动到底部
        ScrollToBottom();
    }
    
    private void Message_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // 当消息内容变化时（流式更新），滚动到底部
        if (e.PropertyName == nameof(AvaAIChat.Models.ChatMessageModel.Content))
        {
            ScrollToBottom();
        }
    }
    
    private void ScrollToBottom()
    {
        // 标记需要滚动，由定时器执行
        _needsScroll = true;
    }
    
    private void PerformScroll()
    {
        // 实际执行滚动
        if (MessageScrollViewer != null)
        {
            MessageScrollViewer.ScrollToEnd();
        }
    }
}
