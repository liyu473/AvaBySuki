using Avalonia.Controls;
using Avalonia.Media;
using ColorPicker.Models;

namespace AvaBySuki.Controls;

public partial class CustomColorDialog : UserControl
{
    public CustomColorDialog()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 获取当前选择的颜色
    /// </summary>
    public Color GetSelectedColor()
    {
        if (ColorPickerControl.Color is NotifyableColor notifyableColor)
        {
            return Color.FromArgb(255,
                (byte)notifyableColor.RGB_R,
                (byte)notifyableColor.RGB_G,
                (byte)notifyableColor.RGB_B);
        }
        return Colors.DeepSkyBlue;
    }
}
