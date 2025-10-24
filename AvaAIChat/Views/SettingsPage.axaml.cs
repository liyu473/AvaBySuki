using Avalonia.Controls;
using AvaAIChat.ViewModels;

namespace AvaAIChat.Views;

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