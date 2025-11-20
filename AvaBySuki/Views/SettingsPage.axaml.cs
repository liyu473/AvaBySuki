using Avalonia.Controls;
using AvaBySuki.ViewModels;

namespace AvaBySuki.Views;

public partial class SettingsPage : UserControl
{
    private readonly SettingsViewModel _vm;

    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        _vm = vm;
    }
}