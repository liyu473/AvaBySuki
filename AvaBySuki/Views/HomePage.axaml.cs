using Avalonia.Controls;
using AvaBySuki.ViewModels;
using LyuExtensions.Aspects;

namespace AvaBySuki.Views;

[Transient]
public partial class HomePage : UserControl
{

    [Inject]
    private readonly HomePageViewModel _vm;

    public HomePage() 
    {
        InitializeComponent();
        DataContext = _vm;
    }
}