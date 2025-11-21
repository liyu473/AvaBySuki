using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WebViewControl;

namespace AvaBySuki.ViewModels;

public partial class WebPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Address { get; set; } = "https://github.com/liyu473/AvaBySuki";
    
    [RelayCommand]
    private void ShowTools(WebView webview)
    {
        webview.ShowDeveloperTools();
    }
    
    [RelayCommand]
    private void GoBack(WebView webview)
    {
        webview.GoBack();
    }
    
    [RelayCommand]
    private void GoForward(WebView webview)
    {
        webview.GoForward();
    }
}