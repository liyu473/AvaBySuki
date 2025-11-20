using Avalonia.Controls;
using AvaBySuki.ViewModels;

namespace AvaBySuki.Views;

public partial class HomePage : UserControl
{
    public HomePage()
    {
        InitializeComponent();
    }
    
    public HomePage(HomePageViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}