using Avalonia.Controls;
using AvaBySuki.ViewModels;
using LyuExtensions.Aspects;

namespace AvaBySuki.Views;

[Transient]
public partial class SettingsPage : UserControl
{
    [Inject]
    private readonly SettingsViewModel? _vm;
   
    
    public SettingsPage() 
    {
        InitializeComponent();
        DataContext = _vm;
    }
}