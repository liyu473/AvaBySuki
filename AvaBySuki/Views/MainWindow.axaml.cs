using AvaBySuki.ViewModels;
using LyuExtensions.Aspects;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace AvaBySuki.Views;

[Transient]
public partial class MainWindow : SukiWindow
{
    [Inject]
    private readonly MainWindowViewModel? _vm;

    public MainWindow(ISukiDialogManager dialogManager, ISukiToastManager toastManager)
    {
        InitializeComponent();

        DialogHost.Manager = dialogManager;
        ToastHost.Manager = toastManager;

        DataContext = _vm;
    }
}
