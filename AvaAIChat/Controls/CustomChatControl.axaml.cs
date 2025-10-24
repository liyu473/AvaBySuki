using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using AvaAIChat.ViewModels;

namespace AvaAIChat.Controls;

public partial class CustomChatControl : UserControl
{
    private DispatcherTimer? _scrollTimer;
    private bool _needsScroll;
    private bool _isUserScrolling = false;
    private double _lastScrollOffset = 0;
    
    public CustomChatControl()
    {
        InitializeComponent();
        
        // 监听数据上下文变化
        DataContextChanged += OnDataContextChanged;
        
        // 监听滚动事件，检测用户是否在手动滚动
        if (MessageScrollViewer != null)
        {
            MessageScrollViewer.ScrollChanged += OnScrollChanged;
        }
        
        // 初始化滚动定时器（节流）
        _scrollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _scrollTimer.Tick += (s, e) =>
        {
            if (_needsScroll && !_isUserScrolling)
            {
                PerformScroll();
                _needsScroll = false;
            }
        };
        _scrollTimer.Start();
    }
    
    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (MessageScrollViewer == null) return;
        
        var currentOffset = MessageScrollViewer.Offset.Y;
        var maxOffset = MessageScrollViewer.Extent.Height - MessageScrollViewer.Viewport.Height;
        
        // 如果用户向上滚动（手动滚动），停止自动滚动
        if (currentOffset < _lastScrollOffset && currentOffset < maxOffset - 50)
        {
            _isUserScrolling = true;
        }
        // 如果用户滚动到底部附近，恢复自动滚动
        else if (maxOffset - currentOffset < 50)
        {
            _isUserScrolling = false;
        }
        
        _lastScrollOffset = currentOffset;
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
