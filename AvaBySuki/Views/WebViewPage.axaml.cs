using AvaBySuki.ViewModels;
using Avalonia.Controls;
using LyuExtensions.Aspects;

namespace AvaBySuki.Views;

[Transient]
public partial class WebViewPage : UserControl
{
    [Inject]
    private readonly WebPageViewModel _vm;

    public WebViewPage()
    {
        InitializeComponent();
        DataContext = _vm;
    }
}
