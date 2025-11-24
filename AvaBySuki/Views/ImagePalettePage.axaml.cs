using Avalonia.Controls;
using AvaBySuki.ViewModels;

namespace AvaBySuki.Views;

public partial class ImagePalettePage : UserControl
{
    public ImagePalettePage()
    {
        InitializeComponent();
    }
    
    public ImagePalettePage(ImagePaletteViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
