using Avalonia.Controls;
using AvaAIChat.ViewModels;

namespace AvaAIChat.Views;

public partial class HomePage : UserControl
{
    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}