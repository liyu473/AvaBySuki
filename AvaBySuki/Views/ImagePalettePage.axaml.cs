using Avalonia.Controls;
using AvaBySuki.ViewModels;
using LyuExtensions.Aspects;

namespace AvaBySuki.Views;

[Transient]
public partial class ImagePalettePage : UserControl
{
    [Inject]
    private readonly ImagePaletteViewModel _vm;

    public ImagePalettePage()
    {
        InitializeComponent();
        DataContext = _vm;
    }
}
