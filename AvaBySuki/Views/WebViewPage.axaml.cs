using AvaBySuki.ViewModels;
using Avalonia.Controls;
using System;

namespace AvaBySuki.Views;

public partial class WebViewPage : UserControl
{
    private readonly WebPageViewModel _vm;

    public WebViewPage()
    {
        InitializeComponent();
    }

    public WebViewPage(WebPageViewModel vm)
        : this()
    {
        DataContext = vm;
        _vm = vm;
    }
}
