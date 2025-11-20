using Avalonia.Controls;
using AvaBySuki.ViewModels;

namespace AvaBySuki.Views;

public partial class HomePage : UserControl
{
    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}