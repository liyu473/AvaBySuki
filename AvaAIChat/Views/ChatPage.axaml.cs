using AvaAIChat.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace AvaAIChat.Views;

public partial class ChatPage : UserControl
{
    private readonly ChatViewModel _vm;
    
    public ChatPage(ChatViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        _vm = vm;
    }
}