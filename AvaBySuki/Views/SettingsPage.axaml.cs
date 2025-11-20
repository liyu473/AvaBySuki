using Avalonia.Controls;
using AvaBySuki.ViewModels;

namespace AvaBySuki.Views;

public partial class SettingsPage : UserControl
{
    private readonly SettingsViewModel? _vm;
    
    public SettingsPage()
    {
        InitializeComponent();
    }
    
    public SettingsPage(SettingsViewModel vm) : this()
    {
        DataContext = vm;
        _vm = vm;
    }
}