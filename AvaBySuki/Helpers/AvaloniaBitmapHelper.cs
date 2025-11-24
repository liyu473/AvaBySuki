using Avalonia.Media.Imaging;
using PaletteNet;

namespace AvaBySuki.Helpers;

/// <summary>
/// Avalonia Bitmap 到 PaletteNet 的适配器
/// </summary>
public class AvaloniaBitmapHelper : IBitmapHelper
{
    private readonly WriteableBitmap _bitmap;

    public AvaloniaBitmapHelper(Bitmap sourceBitmap)
    {
        // 转换为 WriteableBitmap 以便访问像素数据
        _bitmap = new WriteableBitmap(
            new Avalonia.PixelSize(sourceBitmap.PixelSize.Width, sourceBitmap.PixelSize.Height),
            new Avalonia.Vector(96, 96),
            Avalonia.Platform.PixelFormat.Bgra8888,
            Avalonia.Platform.AlphaFormat.Premul
        );

        using var lockedBitmap = _bitmap.Lock();
        sourceBitmap.CopyPixels(
            new Avalonia.PixelRect(0, 0, sourceBitmap.PixelSize.Width, sourceBitmap.PixelSize.Height),
            lockedBitmap.Address,
            lockedBitmap.RowBytes * sourceBitmap.PixelSize.Height,
            lockedBitmap.RowBytes
        );
    }

    public int Width => _bitmap.PixelSize.Width;

    public int Height => _bitmap.PixelSize.Height;

    public int[] GetPixels()
    {
        var pixels = new int[Width * Height];
        
        using var lockedBitmap = _bitmap.Lock();
        unsafe
        {
            var ptr = (byte*)lockedBitmap.Address.ToPointer();
            var stride = lockedBitmap.RowBytes;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var offset = y * stride + x * 4;
                    var b = ptr[offset];
                    var g = ptr[offset + 1];
                    var r = ptr[offset + 2];
                    var a = ptr[offset + 3];

                    // 转换为 ARGB 格式的 int 值
                    pixels[y * Width + x] = (a << 24) | (r << 16) | (g << 8) | b;
                }
            }
        }

        return pixels;
    }

    public int[] ScaleDownAndGetPixels()
    {
        // 简单实现，直接返回原始像素
        // 如果图片太大，PaletteNet 会自动处理
        return GetPixels();
    }
}
