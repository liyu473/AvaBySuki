using AvaBySuki.ViewModels;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace AvaBySuki.Views;

public partial class MainWindow : SukiWindow
{
    private readonly MainWindowViewModel _vm;

    public MainWindow(
        MainWindowViewModel vm,
        ISukiDialogManager dialogManager,
        ISukiToastManager toastManager)
    {
        InitializeComponent();
        
        DialogHost.Manager = dialogManager;
        ToastHost.Manager = toastManager;

        DataContext = vm;
        _vm = vm;
    }
}